namespace synthesis.api.Services.OpenAi;

using Azure.AI.OpenAI;
using synthesis.api.Data.Models;
using synthesis.api.Services.OpenAi.Dtos;
using System.Text.Json;

public interface IChatGptService
{
    Task<GptProjectDto> GenerateProject(string prompt);

}
public class ChatGptService : IChatGptService
{

    private OpenAIClient _gpt4Client;
    private OpenAIClient _gpt3TurboClient;

    public ChatGptService()
    {
        _gpt3TurboClient = GptClients.GPT3Turbo();
        _gpt4Client = GptClients.GPT4();
    }

    public async Task<GptProjectDto> GenerateProject(string prompt)
    {
        var overview = GetProjectOverview(prompt);
        var competitiveAnalysis = GetProjectCompetitiveAnalysis(prompt);
        var features = GetProjectFeatures(prompt);

        await Task.WhenAll(overview, competitiveAnalysis, features);


        var projectMetadata = new GptProjectDto()
        {
            Overview = overview.Result,
            CompetitiveAnalysis = competitiveAnalysis.Result,
            Features = features.Result
        };

        return projectMetadata;
    }

    private async Task<Overview> GetProjectOverview(string prompt)
    {
        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = GptClients._GPT4Deployment,
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

        var responseWithoutStream = await _gpt4Client.GetChatCompletionsAsync(ChatCompletionOptions);

        var response = responseWithoutStream.Value;

        var overview = JsonSerializer.Deserialize<Overview>(response.Choices[0].Message.Content);

        return overview ?? new Overview();

    }

    private async Task<List<GptFeatureDto>> GetProjectFeatures(string prompt)
    {
        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = GptClients._GPT3TurboDeployment,
            Messages =
            {
                new ChatRequestAssistantMessage(GptSystemMessage.GetFeatures),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = (float)0.8,
            MaxTokens = 4000,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };


        var responseWithoutStream = await _gpt3TurboClient.GetChatCompletionsAsync(ChatCompletionOptions);

        var response = responseWithoutStream.Value;

        var features = JsonSerializer.Deserialize<List<GptFeatureDto>>(response.Choices[0].Message.Content);

        return features;

    }

    private async Task<CompetitiveAnalysis> GetProjectCompetitiveAnalysis(string prompt)
    {
        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = GptClients._GPT4Deployment,
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

        var responseWithoutStream = await _gpt4Client.GetChatCompletionsAsync(ChatCompletionOptions);

        var response = responseWithoutStream.Value;

        var competitiveAnalysis = JsonSerializer.Deserialize<CompetitiveAnalysis>(response.Choices[0].Message.Content);

        return competitiveAnalysis ?? new CompetitiveAnalysis();
    }

    private async Task<GenerateBrandingDto> GetProjectBranding(string prompt)
    {
        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = GptClients._GPT3TurboDeployment,
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

        var responseWithoutStream = await _gpt3TurboClient.GetChatCompletionsAsync(ChatCompletionOptions);

        var response = responseWithoutStream.Value;

        var brandingResponse = JsonSerializer.Deserialize<GenerateBrandingDto>(response.Choices[0].Message.Content);

        return brandingResponse;
    }

}
