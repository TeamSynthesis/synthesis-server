using synthesis.api.Services.Tinify;

namespace synthesis.api.Services.OpenAi;

using Azure.AI.OpenAI;
using Octokit;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Mappings;
using synthesis.api.Services.OpenAi.Dtos;
using System.Text.Json;

public interface IChatGptService
{
    Task<GlobalResponse<GeneratedPrePlanDto>> GeneratePreplan(string prompt, List<GptTeamMemberDto> teamMembers);

}
public class ChatGptService : IChatGptService
{

    private readonly OpenAIClient _gpt4Client;
    private readonly OpenAIClient _gpt3TurboClient;
    private readonly OpenAIClient _gpt3FCClient;
    private readonly OpenAIClient _gpt4FCClient;
    private readonly IDalleService _dalleService;
    private readonly IImageOptimizerService _optimizerService;

    private readonly RepositoryContext _repository;

    public ChatGptService(IImageOptimizerService optimizerService)
    {
        _optimizerService = optimizerService;

        _gpt3TurboClient = GptClients.GPT3Turbo();
        _gpt3FCClient = GptClients.GPT3FC();

        _gpt4Client = GptClients.GPT4();
        _gpt4FCClient = GptClients.GPT4FC();

        _dalleService = new DalleService(_optimizerService);
    }

    public async Task<GlobalResponse<GeneratedPrePlanDto>> GeneratePreplan(string prompt, List<GptTeamMemberDto> teamMembers)
    {
        //these variable are keeping the tasks which are to be triggered to generate the
        //respective parts of the plan
        var overview = GetProjectOverview(prompt);
        var competitiveAnalysis = GetProjectCompetitiveAnalysis(prompt);
        var features = GetProjectFeatures(prompt, teamMembers);
        var branding = GetProjectBranding(prompt);

        //intialize the generation of all parts of the preplan simultaneously  
        await Task.WhenAll(overview, competitiveAnalysis, features, branding);

        //map the  componeent parts of the preplan into one GeneratedPrePlanDto Object 
        var preplan = new GeneratedPrePlanDto()
        {
            Overview = overview.Result,
            CompetitiveAnalysis = competitiveAnalysis.Result,
            Features = features.Result,
            Branding = branding.Result,

        };

        //if the preplan is null then return an error
        if (preplan == null)
        {
            return new GlobalResponse<GeneratedPrePlanDto>(false, "generate project failed", errors: ["something went wrong"]);
        }

        //else return the mapped preplan
        return new GlobalResponse<GeneratedPrePlanDto>(true, "project generated successfully", preplan);
    }

    private async Task<Overview> GetProjectOverview(string prompt)
    {
        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = GptClients._GPT4Deployment,
            Messages =
            {
                new ChatRequestAssistantMessage(GptSystemMessages.GetOverviewPrompt()),
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

    private async Task<List<GptFeatureDto>> GetProjectFeatures(string prompt, List<GptTeamMemberDto> teamMembers)
    {

        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = GptClients._GPT4FCDeployment,
            Messages =
            {
                new ChatRequestAssistantMessage(GptSystemMessages.GetFeaturesPrompt(teamMembers)),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = (float)0.8,
            MaxTokens = 4000,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };


        var responseWithoutStream = await _gpt4FCClient.GetChatCompletionsAsync(ChatCompletionOptions);

        var response = responseWithoutStream.Value;

        var features = JsonSerializer.Deserialize<List<GptFeatureDto>>(response.Choices[0].Message.Content);

        return features ?? new List<GptFeatureDto>();

    }

    private async Task<CompetitiveAnalysis> GetProjectCompetitiveAnalysis(string prompt)
    {
        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = GptClients._GPT4FCDeployment,
            Messages =
            {
                new ChatRequestAssistantMessage(GptSystemMessages.GetProjectCompetitiveAnalysisPrompt()),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = (float)0.7,
            MaxTokens = 4000,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };

        var responseWithoutStream = await _gpt4FCClient.GetChatCompletionsAsync(ChatCompletionOptions);

        var response = responseWithoutStream.Value;

        var competitiveAnalysis = JsonSerializer.Deserialize<CompetitiveAnalysis>(response.Choices[0].Message.Content);

        return competitiveAnalysis ?? new CompetitiveAnalysis();
    }

    private async Task<Branding> GetProjectBranding(string prompt)
    {
        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = GptClients._GPT4FCDeployment,
            Messages =
            {
                new ChatRequestAssistantMessage(GptSystemMessages.GetBrandingPrompt()),
                new ChatRequestUserMessage(prompt)
            },
            Temperature = (float)0.8,
            MaxTokens = 4000,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };

        var responseWithoutStream = await _gpt4FCClient.GetChatCompletionsAsync(ChatCompletionOptions);

        var response = responseWithoutStream.Value;

        var brandingResponse = JsonSerializer.Deserialize<Branding>(response.Choices[0].Message.Content);

        List<string> imagePrompts =
        [
            brandingResponse.Icon.Description,
            brandingResponse.Wireframe.Image.Description,
            brandingResponse.MoodBoard.Description,
        ];

        var images = await GenerateImages(imagePrompts);

        var branding = new Branding()
        {
            Icon = brandingResponse.Icon,
            Slogan = brandingResponse.Slogan,
            Wireframe = brandingResponse.Wireframe,
            MoodBoard = brandingResponse.MoodBoard,
            Palette = brandingResponse.Palette,
            Typography = brandingResponse.Typography
        };

        branding.Icon.ImgUrl = images[0];
        branding.Wireframe.Image.ImgUrl = images[1];
        branding.MoodBoard.ImgUrl = images[2];

        return branding;
    }


    private async Task<List<string>> GenerateImages(List<string> imagePrompts)
    {
        var images = new List<string>();

        foreach (var prompt in imagePrompts)
        {
            var image = await _dalleService.GenerateImage(prompt);
            images.Add(image);
        }
        return images;
    }
}
