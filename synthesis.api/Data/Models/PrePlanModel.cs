using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using synthesis.api.Services.OpenAi.Dtos;

namespace synthesis.api.Data.Models;

public class PlanModel
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(TeamModel))]
    public TeamModel? Team { get; set; }
    
    public Guid TeamId { get; set; }
    public PlanStatus Status { get; set; }

    public string? Project { get; set; }

    public bool IsSuccess { get; set; }

}

public enum PlanStatus
{
    inprogress,
    complete,
    failed
}