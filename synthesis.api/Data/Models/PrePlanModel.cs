using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using synthesis.api.Services.OpenAi.Dtos;

namespace synthesis.api.Data.Models;

public class PrePlanModel
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(TeamModel))]
    public Guid TeamId { get; set; }
    public TeamModel? Team { get; set; }
    
    public PlanStatus Status { get; set; }
    
    public string? Plan { get; set; }
    
    public bool IsSuccess { get; set; }

}

public enum PlanStatus
{
    Inprogress,
    Complete,
    Failed
}