using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class UserSessionModel
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(UserModel))]
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
}