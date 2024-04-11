using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class FeatureModel
{
    [Column("FeatureId")]
    public Guid Id { get; set; }

    [ForeignKey(nameof(ProjectModel))]
    public Guid ProjectId { get; set; }
    public ProjectModel? Project { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public FeatureType Type { get; set; }

    public List<TaskToDoModel>? Tasks { get; set; }

}

public enum FeatureType
{
    Must,
    Should,
    Could
}