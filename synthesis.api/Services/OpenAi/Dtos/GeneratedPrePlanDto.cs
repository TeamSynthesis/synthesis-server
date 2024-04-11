using synthesis.api.Data.Models;

namespace synthesis.api.Services.OpenAi.Dtos;

public class GeneratedPrePlanDto
{
    public Overview? Overview { get; set; }
    public CompetitiveAnalysis? CompetitiveAnalysis { get; set; }
    public Branding? Branding { get; set; }
    public Technology? Technology { get; set; }
    public List<GptFeatureDto>? Features { get; set; }
}