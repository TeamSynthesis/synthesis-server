using Azure.AI.OpenAI;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class ProjectModel
{
    [Column("ProjectId")]
    public Guid Id { get; set; }

    [ForeignKey(nameof(TeamModel))]
    public Guid TeamId { get; set; }
    public TeamModel? Team { get; set; }

    public string? Name { get; set; }

    public string? IconUrl { get; set; }

    public List<FeatureModel>? Features { get; set; }

    public List<TaskToDoModel>? Tasks { get; set; }

    [Column(TypeName = "jsonb")]
    public ProjectMetadata? ProjectMetadata { get; set; }
}
