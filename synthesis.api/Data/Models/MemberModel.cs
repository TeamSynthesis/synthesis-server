using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class MemberModel
{
    [Column("MemberId")]
    public Guid Id { get; set; }

    [ForeignKey(nameof(UserModel))]
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }
    public DateTime? JoinedOn { get; set; }
    public List<string>? Roles { get; set; }

    [ForeignKey(nameof(TeamModel))]
    public Guid TeamId { get; set; }
    public TeamModel? Team { get; set; }

    public List<TaskToDoModel>? Tasks { get; set; }

}
