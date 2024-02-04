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
    Task<Response<OrganisationDto>> CreateOrganisation(Guid userId, CreateOrganisationDto organisationRequest);
    Task<Response<OrganisationDto>> GetOrganisationById(Guid id);
    Task<Response<OrganisationDto>> GetOrganisationWithResourcesById(Guid id);
    Task<Response<List<MemberDto>>> GetOrganisationMembers(Guid id);
    Task<Response<OrganisationDto>> UpdateOrganisation(Guid id, UpdateOrganisationDto updateRequest);
    Task<Response<OrganisationDto>> PatchOrganisation(Guid id, UpdateOrganisationDto patchRequest);
    Task<Response<OrganisationDto>> DeleteOrganisation(Guid id);
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

    public async Task<Response<OrganisationDto>> CreateOrganisation(Guid userId, CreateOrganisationDto organisationRequest)
    {
        var userExists = await _repository.Users.AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            return new Response<OrganisationDto>(false, "create organisation failed", errors: [$"user with id: {userId} not found"]);
        }

        var organisation = _mapper.Map<OrganisationModel>(organisationRequest);

        var validationResult = new OrganisationValidator().Validate(organisation);

        if (!validationResult.IsValid)
        {
            return new Response<OrganisationDto>(false, "create organisation failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var member = new MemberModel()
        {
            UserId = userId,
            Roles = [UserRoles.Owner]
        };

        organisation.Members = [member];

        await _repository.Organisations.AddAsync(organisation);

        await _repository.SaveChangesAsync();

        var organisationToReturn = _mapper.Map<OrganisationDto>(organisation);

        return new Response<OrganisationDto>(true, "organisation created", value: organisationToReturn);

    }

    public async Task<Response<OrganisationDto>> GetOrganisationById(Guid id)
    {
        var organisation = await _repository.Organisations.FindAsync(id);

        if (organisation == null)
        {
            return new Response<OrganisationDto>(true, "get organisation failed", errors: [$"organisation with id: {id} not found"]);
        }

        var organisationToReturn = _mapper.Map<OrganisationDto>(organisation);

        return new Response<OrganisationDto>(true, "get organisation success", value: organisationToReturn);
    }

    public async Task<Response<OrganisationDto>> GetOrganisationWithResourcesById(Guid id)
    {
        var organisation = await _repository.Organisations.Where(org => org.Id == id).Include(org => org.Members).ThenInclude(m => m.User).Include(org => org.Projects).SingleOrDefaultAsync();

        if (organisation == null)
        {
            return new Response<OrganisationDto>(true, "get organisation failed", errors: [$"organisation with id: {id} not found"]);
        }

        var organisationToReturn = _mapper.Map<OrganisationDto>(organisation);

        return new Response<OrganisationDto>(true, "get organisation success", value: organisationToReturn);

    }

    public async Task<Response<List<MemberDto>>> GetOrganisationMembers(Guid id)
    {
        var organisation = await _repository.Organisations.AnyAsync(org => org.Id == id);

        if (!organisation)
        {
            return new Response<List<MemberDto>>(false, "get members failed", errors: [$"organisation with id: {id} not found"]);
        }

        var members = _repository.Members.Where(m => m.OrganisationId == id).Include(m => m.User);

        var membersToReturn = _mapper.Map<List<MemberDto>>(members);

        return new Response<List<MemberDto>>(true, "get members success", value: membersToReturn);

    }

    public async Task<Response<List<ProjectDto>>> GetOrganisationProjects(Guid id)
    {
        var organisation = await _repository.Organisations.AnyAsync(org => org.Id == id);

        if (!organisation)
        {
            return new Response<List<ProjectDto>>(false, "get projects failed", errors: [$"organisation with id: {id} not found"]);
        }

        var projects = _repository.Projects.Where(p => p.OrganisationId == id);

        var projectsToReturn = _mapper.Map<List<ProjectDto>>(projects);

        return new Response<List<ProjectDto>>(true, "get projects success", value: projectsToReturn);

    }

    public async Task<Response<OrganisationDto>> UpdateOrganisation(Guid id, UpdateOrganisationDto updateRequest)
    {
        var organisation = await _repository.Organisations.FindAsync(id);

        if (organisation == null)
        {
            return new Response<OrganisationDto>(false, "update organisation failed", errors: [$"organisation with id: {id} not found"]);
        }

        var updatedOrganisation = _mapper.Map(updateRequest, organisation);

        var validationResult = new OrganisationValidator().Validate(updatedOrganisation);

        if (!validationResult.IsValid)
        {
            return new Response<OrganisationDto>(false, "create organisation failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new Response<OrganisationDto>(true, "update organisation success");

    }

    public async Task<Response<OrganisationDto>> PatchOrganisation(Guid id, UpdateOrganisationDto patchRequest)
    {
        var organisation = await _repository.Organisations.FindAsync(id);
        if (organisation == null) return new Response<OrganisationDto>(false, "delete organisation failed", errors: [$"organisation with id{id} not found"]);

        var organisationToBePatched = _mapper.Map<UpdateOrganisationDto>(organisation);

        foreach (var prop in patchRequest.GetType().GetProperties())
        {
            var value = prop.GetValue(patchRequest);

            if (value != null)
            {
                prop.SetValue(organisationToBePatched, value);
            }
        }

        var patchedOrganisation = _mapper.Map(organisationToBePatched, organisation);

        var validationResult = await new OrganisationValidator().ValidateAsync(patchedOrganisation);
        if (!validationResult.IsValid)
        {
            return new Response<OrganisationDto>(false, "update organisation failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new Response<OrganisationDto>(true, "patch organisation success");
    }

    public async Task<Response<OrganisationDto>> DeleteOrganisation(Guid id)
    {
        var organisation = await _repository.Organisations.FindAsync(id);

        if (organisation == null)
        {
            return new Response<OrganisationDto>(false, "update organisation failed", errors: [$"organisation with id: {id} not found"]);
        }

        _repository.Organisations.Remove(organisation);

        await _repository.SaveChangesAsync();

        return new Response<OrganisationDto>(true, "delete organisation success");
    }
}