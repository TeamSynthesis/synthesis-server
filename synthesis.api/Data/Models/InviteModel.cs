using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class InviteModel
{
    [Key]
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Email { get; set; }
    public DateTime InvitedOn { get; set; }
    public string? Role { get; set; }
    public bool Accepted { get; set; }

    [ForeignKey(nameof(TeamModel))]
    public Guid TeamId { get; set; }
    public TeamModel? Team { get; set; }

}