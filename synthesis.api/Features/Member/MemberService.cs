using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

public interface IMemberService
{
    Task<Response<MemberDto>> CreateMemberProfile(Guid organisationId, Guid userId);
    Task<Response<MemberDto>> GetMemberProfileById(Guid id);
    Task<Response<MemberDto>> AssignMemberRole(Guid id, string role);
    Task<Response<MemberDto>> ResignMemberRole(Guid id, string role);
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

    public async Task<Response<MemberDto>> GetMemberProfileById(Guid id)
    {
        var member = await _repository.Members.Where(m => m.Id == id).Include(m => m.User).SingleOrDefaultAsync();

        if (member == null) return new Response<MemberDto>(false, "get member profile failed", errors: [$"member with id: {id} not found"]);

        var memberToReturn = _mapper.Map<MemberDto>(member);

        return new Response<MemberDto>(true, "get member profile success", value: memberToReturn);
    }

    public async Task<Response<MemberDto>> AssignMemberRole(Guid id, string role)
    {
        var member = await _repository.Members.Where(m => m.Id == id).Include(m => m.User).SingleOrDefaultAsync();

        if (member == null) return new Response<MemberDto>(false, "get member profile failed", errors: [$"member with id: {id} not found"]);

        switch (role)
        {
            case "manager":
                if (member.Roles == null)
                {
                    member.Roles = [role];
                }
                else
                {
                    if (member.Roles.Contains(role)) return new Response<MemberDto>(false, "assign member role failed", errors: ["duplicate role assignment"]);
                    member.Roles.Add(role);
                }
                break;
            case "owner":
                if (member.Roles == null)
                {
                    member.Roles = [role];
                }
                else
                {
                    if (member.Roles.Contains(role)) return new Response<MemberDto>(false, "assign member role failed", errors: ["duplicate role assignment"]);
                    member.Roles.Add(role);
                }
                break;
            default:
                return new Response<MemberDto>(false, "assign role failed", errors: ["member role invalid [Roles : manager || owner]"]);
        }

        await _repository.SaveChangesAsync();

        return new Response<MemberDto>(true, "assign role success");

    }

    public async Task<Response<MemberDto>> ResignMemberRole(Guid id, string role)
    {
        var member = await _repository.Members.Where(m => m.Id == id).Include(m => m.User).SingleOrDefaultAsync();

        if (member == null) return new Response<MemberDto>(false, "get member profile failed", errors: [$"member with id: {id} not found"]);

        if (member.Roles == null || !member.Roles.Contains(role))
        {
            return new Response<MemberDto>(false, "resign member role failed", errors: [$"member does not possess the role: {role}"]);
        }
        else
        {
            member.Roles.Remove(role);
        }

        await _repository.SaveChangesAsync();

        return new Response<MemberDto>(true, "resign member role success");

    }
}
