using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class TeamModel
{
    [Column("TeamId")]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public DateTime? CreatedOn { get; set; }
    public int SeatsAvailable { get; set; }
    public List<string>? Invites { get; set; }
    public string? AvatarUrl { get; set; }
    public List<MemberModel>? Members { get; set; }
    public List<ProjectModel>? Projects { get; set; }

}