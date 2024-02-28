using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class UserModel
{
    [Column("UserId")]
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? Profession { get; set; }
    public string? AvatarUrl { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public int? GitHubId { get; set; }
    public bool EmailConfirmed { get; set; }
    public OnBoardingProgress OnBoarding { get; set; }
    public List<string>? Skills { get; set; }


    public List<MemberModel>? MemberProfiles { get; set; }
    public List<RefreshTokenModel>? RefreshTokens { get; set; }

}

public enum OnBoardingProgress
{
    CreateAccount,
    Details,
    Skills
}
