using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Organisation;

public interface IOrganisationService
{
    Task<Response<OrganisationDto>> CreateOrganisation(Guid userId, CreateOrganisationDto organisationRequest);
    Task<Response<OrganisationDto>> GetOrganisationById(Guid id);
    Task<Response<List<MemberDto>>> GetOrganisationMembers(Guid id);

    Task<Response<OrganisationDto>> UpdateOrganisation(Guid id, UpdateOrganisationDto updateRequest);
}

public class OrganisationService : IOrganisationService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<OrganisationModel> _validator;

    public OrganisationService(RepositoryContext repository, IMapper mapper, IValidator<OrganisationModel> validator)
    {
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Response<OrganisationDto>> CreateOrganisation(Guid userId, CreateOrganisationDto organisationRequest)
    {
        var userExists = await _repository.Users.AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            return new Response<OrganisationDto>(false, "create organisation failed", errors: [$"user with id: {userId} not found"]);
        }

        var organisation = _mapper.Map<OrganisationModel>(organisationRequest);

        var validationResult = _validator.Validate(organisation);

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

    public async Task<Response<OrganisationDto>> UpdateOrganisation(Guid id, UpdateOrganisationDto updateRequest)
    {
        var organisation = await _repository.Organisations.FindAsync(id);

        if (organisation == null)
        {
            return new Response<OrganisationDto>(false, "update organisation failed", errors: [$"organisation with id: {id} not found"]);
        }

        var updatedOrganisation = _mapper.Map(updateRequest, organisation);

        var validationResult = _validator.Validate(updatedOrganisation);

        if (!validationResult.IsValid)
        {
            return new Response<OrganisationDto>(false, "create organisation failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new Response<OrganisationDto>(true, "update organisation success");

    }
}