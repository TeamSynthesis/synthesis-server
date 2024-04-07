using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using synthesis.api.Services.OpenAi.Dtos;

namespace synthesis.api.Data.Models;

public class PlanModel
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(TeamModel))]
    public TeamModel? Team { get; set; }
    public Guid TeamId { get; set; }

    public GptProjectDto? Project { get; set; }

    public bool IsSuccess { get; set; }

}