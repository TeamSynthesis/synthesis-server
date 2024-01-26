using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class UserModel
{
    [Column("UserId")]
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }

    public List<MemberModel>? MemberProfiles { get; set; }

}
