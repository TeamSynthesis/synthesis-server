namespace synthesis.api.Services.OpenAi;

using Azure;
using Azure.AI.OpenAI;
using synthesis.api.Data.Models;
using synthesis.api.Services.g2;
using synthesis.api.Services.OpenAi.Dtos;
using System.Text.Json;

public interface IChatGptService
{
    Task<Overview> GetProjectOverview(string prompt);
    Task<CompetitiveAnalysis> GetProjectCompetitiveAnalysis(string prompt);
    Task<ProjectMetadata> GenerateProjectMetaData(string prompt);
    Task<GenerateBrandingDto> GetProjectBranding(string prompt);
}
public class ChatGptService : IChatGptService
{

    private static readonly string _OpenAiEndpoint = "https://synthesis-ai.openai.azure.com/";
    private static readonly string _OpenAiKey = "cb53dc167aee459292e6e4cff7573476";
    private static readonly string _DeploymentName = "synthesis-gpt4";
    private IG2Service _g2Service;
    private OpenAIClient _client;

    public ChatGptService(IG2Service g2Service)
    {
        _g2Service = g2Service;
        _client = new OpenAIClient(new Uri(_OpenAiEndpoint), new AzureKeyCredential(_OpenAiKey));
    }

    public async Task<ProjectMetadata> GenerateProjectMetaData(string prompt)
    {
        var overview = GetProjectOverview(prompt);
        var competitiveAnalysis = GetProjectCompetitiveAnalysis(prompt);
        // var features = GetProjectFeatures(prompt);

        await Task.WhenAll(overview, competitiveAnalysis);

        var projectMetadata = new ProjectMetadata()
        {
            Overview = overview.Result,
            CompetitiveAnalysis = competitiveAnalysis.Result,
            // Features = features.Resultl
        };

        return projectMetadata;
    }

    public async Task<Overview> GetProjectOverview(string prompt)
    {
        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = _DeploymentName,
            Messages =
            {
                new ChatRequestAssistantMessage(GptSystemMessage.GetOveview),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = (float)0.8,
            MaxTokens = 3000,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };

        var responseWithoutStream = await _client.GetChatCompletionsAsync(ChatCompletionOptions);

        var response = responseWithoutStream.Value;

        var overview = JsonSerializer.Deserialize<Overview>(response.Choices[0].Message.Content);

        return overview ?? new Overview();

    }

    // public async Task<Features> GetProjectFeatures(string prompt)
    // {
    //     var ChatCompletionOptions = new ChatCompletionsOptions()
    //     {
    //         DeploymentName = _DeploymentName,
    //         Messages =
    //         {
    //             new ChatRequestAssistantMessage(GptSystemMessage.GetFeatures),
    //             new ChatRequestUserMessage(prompt)
    //         },
    //         Temperature = (float)0.8,
    //         MaxTokens = 4000,
    //         NucleusSamplingFactor = (float)0.95,
    //         FrequencyPenalty = 0,
    //         PresencePenalty = 0,
    //     };


    //     var responseWithoutStream = await _client.GetChatCompletionsAsync(ChatCompletionOptions);

    //     var response = responseWithoutStream.Value;

    //     var features = JsonSerializer.Deserialize<Features>(response.Choices[0].Message.Content);

    //     return features ?? new Features();
    // }

    public async Task<CompetitiveAnalysis> GetProjectCompetitiveAnalysis(string prompt)
    {
        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = _DeploymentName,
            Messages =
            {
                new ChatRequestAssistantMessage(GptSystemMessage.GetProjectCompetitiveAnalysis),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = (float)0.7,
            MaxTokens = 4000,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };

        var responseWithoutStream = await _client.GetChatCompletionsAsync(ChatCompletionOptions);

        var response = responseWithoutStream.Value;

        var competitiveAnalysis = JsonSerializer.Deserialize<CompetitiveAnalysis>(response.Choices[0].Message.Content);

        return competitiveAnalysis ?? new CompetitiveAnalysis();
    }

    public async Task<GenerateBrandingDto> GetProjectBranding(string prompt)
    {
        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = _DeploymentName,
            Messages =
            {
                new ChatRequestAssistantMessage(GptSystemMessage.GetBranding),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = (float)0.8,
            MaxTokens = 4000,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };

        var responseWithoutStream = await _client.GetChatCompletionsAsync(ChatCompletionOptions);

        var response = responseWithoutStream.Value;

        var brandingResponse = JsonSerializer.Deserialize<GenerateBrandingDto>(response.Choices[0].Message.Content);

        return brandingResponse;
    }

}
