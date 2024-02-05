using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class TeamModel
{
    [Column("TeamId")]
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    [ForeignKey(nameof(ProjectModel))]
    public Guid ProjectId { get; set; }
    public ProjectModel? Project { get; set; }

    public List<MemberModel>? Developers { get; set; }


}