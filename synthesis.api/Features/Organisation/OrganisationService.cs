using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.Project;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Organisation;

public interface IOrganisationService
{
    Task<GlobalResponse<OrganisationDto>> CreateOrganisation(Guid userId, CreateOrganisationDto organisationRequest);
    Task<GlobalResponse<MemberDto>> AddMember(Guid id, Guid userId);
    Task<GlobalResponse<OrganisationDto>> GetOrganisationById(Guid id);
    Task<GlobalResponse<OrganisationDto>> GetOrganisationWithResourcesById(Guid id);
    Task<GlobalResponse<List<MemberDto>>> GetOrganisationMembers(Guid id);
    Task<GlobalResponse<List<ProjectDto>>> GetOrganisationProjects(Guid id);
    Task<GlobalResponse<OrganisationDto>> UpdateOrganisation(Guid id, UpdateOrganisationDto updateRequest);
    Task<GlobalResponse<OrganisationDto>> PatchOrganisation(Guid id, UpdateOrganisationDto patchRequest);
    Task<GlobalResponse<OrganisationDto>> DeleteOrganisation(Guid id);

}

public class OrganisationService : IOrganisationService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;

    public OrganisationService(RepositoryContext repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<GlobalResponse<OrganisationDto>> CreateOrganisation(Guid userId, CreateOrganisationDto organisationRequest)
    {
        var user = await _repository.Users.FindAsync(userId);

        if (user == null)
        {
            return new GlobalResponse<OrganisationDto>(false, "create organisation failed", errors: [$"user with id: {userId} not found"]);
        }

        var organisation = _mapper.Map<OrganisationModel>(organisationRequest);

        var validationResult = new OrganisationValidator().Validate(organisation);

        if (!validationResult.IsValid)
        {
            return new GlobalResponse<OrganisationDto>(false, "create organisation failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var member = new MemberModel()
        {
            User = user,
            Roles = [UserRoles.Owner]
        };

        organisation.Members = [member];

        await _repository.Organisations.AddAsync(organisation);

        await _repository.SaveChangesAsync();

        var organisationToReturn = _mapper.Map<OrganisationDto>(organisation);

        return new GlobalResponse<OrganisationDto>(true, "organisation created", value: organisationToReturn);

    }

    public async Task<GlobalResponse<MemberDto>> AddMember(Guid id, Guid userId)
    {
        var organisation = await _repository.Organisations.FindAsync(id);

        if (organisation == null)
        {
            return new GlobalResponse<MemberDto>(false, "add member to organisation failed", errors: [$"organisation with id {id} not found"]);
        }

        var user = await _repository.Users.FindAsync(userId);

        if (user == null)
        {
            return new GlobalResponse<MemberDto>(false, "add member to organisation failed", errors: [$"user with id {userId} not found"]);
        }

        var memberExists = await _repository.Members.AnyAsync(m => m.UserId == userId);
        if (memberExists)
        {
            return new GlobalResponse<MemberDto>(false, "add member to organisation failed", errors: [$"member with id {userId} already exists"]);
        }

        var member = new MemberModel()
        {
            User = user,
            Organisation = organisation
        };

        await _repository.Members.AddAsync(member);
        await _repository.SaveChangesAsync();


        var memberToReturn = _mapper.Map<MemberDto>(member);

        return new GlobalResponse<MemberDto>(true, "add member to organisation success", value: memberToReturn);
    }

    public async Task<GlobalResponse<OrganisationDto>> GetOrganisationById(Guid id)
    {
        var organisation = await _repository.Organisations
            .Where(org => org.Id == id)
            .Select(x => new OrganisationDto
            {
                Id = x.Id,
                Name = x.Name,
                LogoUrl = x.LogoUrl
            }
            ).SingleOrDefaultAsync();
        if (organisation == null)
        {
            return new GlobalResponse<OrganisationDto>(true, "get organisation failed", errors: [$"organisation with id: {id} not found"]);
        }


        return new GlobalResponse<OrganisationDto>(true, "get organisation success", value: organisation);
    }

    public async Task<GlobalResponse<OrganisationDto>> GetOrganisationWithResourcesById(Guid id)
    {
        var organisation = await _repository.Organisations.Where(org => org.Id == id)
        .Select(org => new OrganisationDto()
        {
            Id = org.Id,
            Name = org.Name,
            LogoUrl = org.LogoUrl,
            Members = org.Members.Select(x => new MemberDto
            {
                Id = x.Id,
                User = new UserDto()
                {
                    Id = x.User.Id,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Username = x.User.UserName,
                    AvatarUrl = x.User.AvatarUrl,
                    Email = x.User.AvatarUrl

                },
                Roles = x.Roles
            }).ToList(),
            Projects = org.Projects.Select(x => new ProjectDto
            {
                Id = x.Id

            }).ToList()
        }).
        SingleOrDefaultAsync();

        if (organisation == null)
        {
            return new GlobalResponse<OrganisationDto>(true, "get organisation failed", errors: [$"organisation with id: {id} not found"]);
        }


        return new GlobalResponse<OrganisationDto>(true, "get organisation success", value: organisation);

    }

    public async Task<GlobalResponse<List<MemberDto>>> GetOrganisationMembers(Guid id)
    {
        var organisationExists = await _repository.Organisations.AnyAsync(org => org.Id == id);

        if (!organisationExists)
        {
            return new GlobalResponse<List<MemberDto>>(false, "get members failed", errors: [$"organisation with id: {id} not found"]);
        }

        var members = await _repository.Members
        .Where(m => m.OrganisationId == id)
        .Select(x => new MemberDto()
        {
            Id = x.Id,
            User = new UserDto()
            {
                Id = x.User.Id,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Username = x.User.UserName,
                Email = x.User.Email,
                AvatarUrl = x.User.AvatarUrl
            },
            Roles = x.Roles
        }).ToListAsync();


        return new GlobalResponse<List<MemberDto>>(true, "get members success", value: members);

    }

    public async Task<GlobalResponse<List<ProjectDto>>> GetOrganisationProjects(Guid id)
    {
        var organisation = await _repository.Organisations.AnyAsync(org => org.Id == id);

        if (!organisation)
        {
            return new GlobalResponse<List<ProjectDto>>(false, "get projects failed", errors: [$"organisation with id: {id} not found"]);
        }

        var projects = await _repository.Projects
            .Where(p => p.OrganisationId == id)
            .Select(x => new ProjectDto
            {
                Id = x.Id
            }).ToListAsync();


        return new GlobalResponse<List<ProjectDto>>(true, "get projects success", value: projects);

    }

    public async Task<GlobalResponse<OrganisationDto>> UpdateOrganisation(Guid id, UpdateOrganisationDto updateRequest)
    {
        var organisation = await _repository.Organisations.FindAsync(id);

        if (organisation == null)
        {
            return new GlobalResponse<OrganisationDto>(false, "update organisation failed", errors: [$"organisation with id: {id} not found"]);
        }

        var updatedOrganisation = _mapper.Map(updateRequest, organisation);

        var validationResult = new OrganisationValidator().Validate(updatedOrganisation);

        if (!validationResult.IsValid)
        {
            return new GlobalResponse<OrganisationDto>(false, "create organisation failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<OrganisationDto>(true, "update organisation success");

    }

    public async Task<GlobalResponse<OrganisationDto>> PatchOrganisation(Guid id, UpdateOrganisationDto patchRequest)
    {
        var organisation = await _repository.Organisations.FindAsync(id);
        if (organisation == null) return new GlobalResponse<OrganisationDto>(false, "delete organisation failed", errors: [$"organisation with id{id} not found"]);

        var organisationToBePatched = _mapper.Map<UpdateOrganisationDto>(organisation);

        var patchedOrganisationDto = Patcher.Patch(patchRequest, organisationToBePatched);

        var patchedOrganisation = _mapper.Map(patchedOrganisationDto, organisation);

        var validationResult = await new OrganisationValidator().ValidateAsync(patchedOrganisation);
        if (!validationResult.IsValid)
        {
            return new GlobalResponse<OrganisationDto>(false, "update organisation failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<OrganisationDto>(true, "patch organisation success");
    }

    public async Task<GlobalResponse<OrganisationDto>> DeleteOrganisation(Guid id)
    {
        var organisation = await _repository.Organisations.FindAsync(id);

        if (organisation == null)
        {
            return new GlobalResponse<OrganisationDto>(false, "update organisation failed", errors: [$"organisation with id: {id} not found"]);
        }

        _repository.Organisations.Remove(organisation);

        await _repository.SaveChangesAsync();

        return new GlobalResponse<OrganisationDto>(true, "delete organisation success");
    }
}