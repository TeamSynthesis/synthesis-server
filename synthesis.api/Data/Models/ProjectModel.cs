using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class ProjectModel
{
    [Column("ProjectId")]
    public Guid Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }

    public DateTime? CreatedOn { get; set; }

    [ForeignKey(nameof(TeamModel))]
    public Guid TeamId { get; set; }
    public TeamModel? Team { get; set; }

    public List<FeatureModel>? Features { get; set; }

    public List<TaskToDoModel>? Tasks { get; set; }

    [ForeignKey(nameof(PrePlanModel))]
    public Guid? PrePlanId { get; set; } 
    public PrePlanModel? PrePlan { get; set; }

    
    
    
}
