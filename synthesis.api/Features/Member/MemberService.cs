using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Mappings;

public interface IMemberService
{
    Task<Response<MemberDto>> CreateMemberProfile(Guid organisationId, Guid userId);
}

public class MemberService : IMemberService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;

    public MemberService(RepositoryContext repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<MemberDto>> CreateMemberProfile(Guid organisationId, Guid userId)
    {
        var organisationExists = await _repository.Organisations.AnyAsync(org => org.Id == organisationId);

        if (!organisationExists)
        {
            return new Response<MemberDto>(false, "create member profile failed", errors: [$"organisation with id {organisationId} not found"]);
        }

        var userExists = await _repository.Users.AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            return new Response<MemberDto>(false, "create member profile failed", errors: [$"user with id {userId} not found"]);
        }

        var member = new MemberModel()
        {
            UserId = userId,
            OrganisationId = organisationId
        };

        await _repository.Members.AddAsync(member);
        await _repository.SaveChangesAsync();


        var memberToReturn = _mapper.Map<MemberDto>(member);

        return new Response<MemberDto>(true, "create member profile success", value: memberToReturn);

    }
}