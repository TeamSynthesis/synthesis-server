
using System.Text.Json;
using synthesis.api.Data.Models;
using synthesis.api.Features.Project;
using synthesis.api.Services.OpenAi.Dtos;

public static class PrePlanDeserializer
{
    public static PrePlanMetaData DeserializePrePlanToMetaData(string prePlanString)
    {
        var deserializedPrePlan = JsonSerializer.Deserialize<GeneratedPrePlanDto>(prePlanString);
        return new PrePlanMetaData
        {
            Overview = deserializedPrePlan.Overview,
            CompetitiveAnalysis = deserializedPrePlan.CompetitiveAnalysis,
            Branding = deserializedPrePlan.Branding,
            Technology = deserializedPrePlan.Technology
        };
    }


    public static GeneratedPrePlanDto DeserializePrePlanToPlanDto(string prePlanString)
    {
        var deserializedPrePlan = JsonSerializer.Deserialize<GeneratedPrePlanDto>(prePlanString);
        return deserializedPrePlan;
    }
}