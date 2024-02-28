using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class TeamModel
{
    [Column("TeamId")]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public List<MemberModel>? Members { get; set; }
    public List<ProjectModel>? Projects { get; set; }

}