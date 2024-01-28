using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class ProjectModel
{
    [Column("ProjectId")]
    public Guid Id { get; set; }

    [ForeignKey(nameof(OrganisationModel))]
    public Guid OrganisationId { get; set; }
    public OrganisationModel? Organisation { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
}