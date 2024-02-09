using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

public interface IMemberService
{
    Task<GlobalResponse<MemberDto>> GetMemberProfileById(Guid id);
    Task<GlobalResponse<MemberDto>> AssignMemberRole(Guid id, string role);
    Task<GlobalResponse<MemberDto>> ResignMemberRole(Guid id, string role);
    Task<GlobalResponse<MemberDto>> DeleteMemberProfile(Guid id);
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

    public async Task<GlobalResponse<MemberDto>> GetMemberProfileById(Guid id)
    {
        var member = await _repository.Members.Where(m => m.Id == id).Include(m => m.User).SingleOrDefaultAsync();

        if (member == null) return new GlobalResponse<MemberDto>(false, "get member profile failed", errors: [$"member with id: {id} not found"]);

        var memberToReturn = _mapper.Map<MemberDto>(member);

        return new GlobalResponse<MemberDto>(true, "get member profile success", value: memberToReturn);
    }

    public async Task<GlobalResponse<MemberDto>> AssignMemberRole(Guid id, string role)
    {
        var member = await _repository.Members.Where(m => m.Id == id).Include(m => m.User).SingleOrDefaultAsync();

        if (member == null) return new GlobalResponse<MemberDto>(false, "get member profile failed", errors: [$"member with id: {id} not found"]);

        switch (role)
        {
            case "manager":
                if (member.Roles == null)
                {
                    member.Roles = [role];
                }
                else
                {
                    if (member.Roles.Contains(role)) return new GlobalResponse<MemberDto>(false, "assign member role failed", errors: ["duplicate role assignment"]);
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
                    if (member.Roles.Contains(role)) return new GlobalResponse<MemberDto>(false, "assign member role failed", errors: ["duplicate role assignment"]);
                    member.Roles.Add(role);
                }
                break;
            default:
                return new GlobalResponse<MemberDto>(false, "assign role failed", errors: ["member role invalid [Roles : manager || owner]"]);
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<MemberDto>(true, "assign role success");

    }

    public async Task<GlobalResponse<MemberDto>> ResignMemberRole(Guid id, string role)
    {
        var member = await _repository.Members.Where(m => m.Id == id).Include(m => m.User).SingleOrDefaultAsync();

        if (member == null) return new GlobalResponse<MemberDto>(false, "get member profile failed", errors: [$"member with id: {id} not found"]);

        if (member.Roles == null || !member.Roles.Contains(role))
        {
            return new GlobalResponse<MemberDto>(false, "resign member role failed", errors: [$"member does not possess the role: {role}"]);
        }
        else
        {
            member.Roles.Remove(role);
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<MemberDto>(true, "resign member role success");

    }

    public async Task<GlobalResponse<MemberDto>> DeleteMemberProfile(Guid id)
    {
        var member = await _repository.Members.FindAsync(id);

        if (member == null) return new GlobalResponse<MemberDto>(false, "delete member profile failed", errors: [$"member with id: {id} not found"]);

        _repository.Members.Remove(member);

        await _repository.SaveChangesAsync();

        return new GlobalResponse<MemberDto>(true, "delete member profile success");
    }
}
