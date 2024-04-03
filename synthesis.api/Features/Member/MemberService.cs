using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

public interface IMemberService
{
    Task<GlobalResponse<MemberDto>> GetMemberProfileById(Guid id);
    Task<GlobalResponse<MemberDto>> AssignMemberRole(Guid id, MemberRole role);
    Task<GlobalResponse<MemberDto>> ResignMemberRole(Guid id, MemberRole role);
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
        var member = await _repository.Members
        .Where(m => m.Id == id)
        .Select(m => new MemberDto
        {
            Id = m.Id,
            User = new UserDto
            {
                Id = m.User.Id,
                FullName = m.User.FullName,
                UserName = m.User.UserName,
                Email = m.User.Email,
                AvatarUrl = m.User.AvatarUrl,
                Profession = m.User.Profession,
                Skills = m.User.Skills
            },
            Roles = m.Roles,
            JoinedOn = m.JoinedOn
        }).FirstOrDefaultAsync();


        return new GlobalResponse<MemberDto>(true, "get member profile success", value: member);
    }

    public async Task<GlobalResponse<MemberDto>> AssignMemberRole(Guid id, MemberRole role)
    {
        var member = await _repository.Members.FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
        {
            return new GlobalResponse<MemberDto>(false, "get member profile failed", errors: [$"member with id: {id} not found"]);
        }

        if (member.Roles != null && member.Roles.Contains(role.GetDisplayName()))
        {
            return new GlobalResponse<MemberDto>(false, "assign member role failed", errors: ["duplicate role assignment"]);
        }

        if (role != MemberRole.owner || role != MemberRole.manager)
        {
            return new GlobalResponse<MemberDto>(false, "assign role failed", errors: ["member role invalid"]);
        }

        member.Roles = [role.GetDisplayName()];

        await _repository.SaveChangesAsync();

        return new GlobalResponse<MemberDto>(true, "assign role success");
    }

    public async Task<GlobalResponse<MemberDto>> ResignMemberRole(Guid id, MemberRole role)
    {
        var member = await _repository.Members.FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
        {
            return new GlobalResponse<MemberDto>(false, "get member profile failed", errors: [$"member with id: {id} not found"]);
        }

        if (member.Roles == null || !member.Roles.Contains(role.GetDisplayName()))
        {
            return new GlobalResponse<MemberDto>(false, "resign member role failed", errors: [$"member does not possess the role: {role}"]);
        }
        else
        {
            member.Roles.Remove(role.GetDisplayName());
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
