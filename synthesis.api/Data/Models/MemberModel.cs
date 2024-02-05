using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class MemberModel
{
    [Column("MemberId")]
    public Guid Id { get; set; }

    [ForeignKey(nameof(UserModel))]
    public Guid UserId { get; set; }
    public UserModel? User { get; set; }

    public List<string>? Roles { get; set; }


    [ForeignKey(nameof(OrganisationModel))]
    public Guid OrganisationId { get; set; }
    public OrganisationModel? Organisation { get; set; }
    
    public List<TeamModel>? Teams { get; set; }
}
