using synthesis.api.Data.Models;

namespace synthesis.api.Services.OpenAi.Dtos;

public class GenerateProjectMetaDataDto
{
    public Overview? Overview { get; set; }
    public CompetitiveAnalysis? CompetitiveAnalysis { get; set; }
    public Branding? Branding { get; set; }
    public Technology? Technology { get; set; }
}


