using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class UserModel
{
    [Column("UserId")]
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? GitHubId { get; set; }
    public bool EmailConfirmed { get; set; }
    public OnBoardingProgress OnBoardingProgress { get; set; }
    public List<Dictionary<string, int>>? Skills { get; set; }
    public List<MemberModel>? MemberProfiles { get; set; }
    public List<UserSessionModel>? Sessions { get; set; }

}

public enum OnBoardingProgress
{
    CreateAccount
}
