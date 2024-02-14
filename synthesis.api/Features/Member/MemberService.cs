using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.Team;
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
        var member = await _repository.Members
        .Where(m => m.Id == id)
        .Select(x => new MemberDto
        {
            Id = x.Id,
            User = new UserDto
            {
                Id = x.User.Id,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Username = x.User.UserName,
                Email = x.User.Email,
                AvatarUrl = x.User.AvatarUrl,
            },
            Organisation = new OrganisationDto
            {
                Id = x.Organisation.Id,
                Name = x.Organisation.Name,
                LogoUrl = x.Organisation.LogoUrl
            },
            Teams = x.Teams.Select(y => new TeamDto
            {
                Id = y.Id,
                Name = y.Name,
                Description = y.Description
            }).ToList()

        }).SingleOrDefaultAsync();


        return new GlobalResponse<MemberDto>(true, "get member profile success", value: member);
    }

    public async Task<GlobalResponse<MemberDto>> AssignMemberRole(Guid id, string role)
    {
        var member = await _repository.Members.FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
        {
            return new GlobalResponse<MemberDto>(false, "get member profile failed", errors: [$"member with id: {id} not found"]);
        }

        if (member.Roles != null && member.Roles.Contains(role))
        {
            return new GlobalResponse<MemberDto>(false, "assign member role failed", errors: ["duplicate role assignment"]);
        }

        if (role != "manager" && role != "owner")
        {
            return new GlobalResponse<MemberDto>(false, "assign role failed", errors: ["member role invalid [Roles : manager || owner]"]);
        }

        member.Roles ??= new List<string>();
        member.Roles.Add(role);

        await _repository.SaveChangesAsync();

        return new GlobalResponse<MemberDto>(true, "assign role success");
    }

    public async Task<GlobalResponse<MemberDto>> ResignMemberRole(Guid id, string role)
    {
        var member = await _repository.Members.FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
        {
            return new GlobalResponse<MemberDto>(false, "get member profile failed", errors: [$"member with id: {id} not found"]);
        }

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
