using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Organisation;

public interface IOrganisationService
{
    Task<Response<OrganisationDto>> CreateOrganisation(Guid userId, CreateOrganisationDto organisationRequest);
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
        var user = await _repository.Users.FindAsync(userId);

        if (user == null)
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
            User = user,
            Roles = [UserRoles.Owner]
        };

        organisation.Members = [member];

        await _repository.Organisations.AddAsync(organisation);

        await _repository.SaveChangesAsync();

        var organisationToReturn = _mapper.Map<OrganisationDto>(organisation);

        return new Response<OrganisationDto>(true, "organisation created", value: organisationToReturn);

    }
}