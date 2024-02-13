/* The code provided is a C# implementation of a service that interacts with the OpenAI GPT-3 API to
generate chat-based completions. */
namespace synthesis.api.Services.OpenAi;

using Azure;
using Azure.AI.OpenAI;
using synthesis.api.Services.g2;
using System.Text.Json;

public interface IChatGptService
{
    Task<ChatCompletions> GenerateProject(string prompt);
}
public class ChatGptService : IChatGptService
{

    private static readonly string _OpenAiEndpoint = "https://manasseh-gpt.openai.azure.com/";
    private static readonly string _OpenAiKey = "5f78e95d6fb946a8b7805822bade4cb4";
    private static readonly string _DeploymentName = "synthesis-gpt4";
    private IG2Service _g2Service;
    private OpenAIClient _client;

    public ChatGptService(IG2Service g2Service)
    {
        _g2Service = g2Service;
        _client = new OpenAIClient(new Uri(_OpenAiEndpoint), new AzureKeyCredential(_OpenAiKey));
    }

    public async Task<ChatCompletions> GenerateProject(string prompt)
    {
        var getCompetitorsFunction = new FunctionDefinition()
        {
            Name = "get_top_competitors",
            Description = "Gets the top competitors for a project proposal idea",
            Parameters = BinaryData.FromObjectAsJson(new
            {
                Type = "object",
                Properties = new
                {
                    Description = new
                    {
                        Type = "string",
                        Description = "very short search keyword description of the proposed app idea"
                    }
                },
                Required = new[] { "Description" }
            },
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            )

        };

        ChatRequestFunctionMessage GetFunctionCallResponseMessage(FunctionCall functionCall)
        {
            if (functionCall?.Name == getCompetitorsFunction.Name)
            {
                string unvalidatedArguments = functionCall.Arguments;
                var functionResultData = _g2Service.GetCompetitors(unvalidatedArguments);

                return new ChatRequestFunctionMessage(functionCall.Name, functionResultData.ToString());
            }
            else throw new NotImplementedException();
        }


        var ChatCompletionOptions = new ChatCompletionsOptions()
        {
            DeploymentName = _DeploymentName,
            Messages =
            {
                new ChatRequestAssistantMessage(GptSystemMessage.SoftwareProjectAssistant),
                new ChatRequestUserMessage(prompt)
            },
            Functions = { getCompetitorsFunction },
            FunctionCall = FunctionDefinition.Auto,
            Temperature = (float)0.6,
            MaxTokens = 4000,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };


        Response<ChatCompletions> responseWithoutStream = await _client.GetChatCompletionsAsync(ChatCompletionOptions);

        ChatCompletions response = responseWithoutStream.Value;

        var reponseChoice = response.Choices[0];

        if (reponseChoice.FinishReason == CompletionsFinishReason.FunctionCall)
        {
            var choice = response.Choices[0];

            ChatCompletionOptions.Messages.Add(GetFunctionCallResponseMessage(choice.Message.FunctionCall));

        }
        else
        {
            return response;
        }

        Response<ChatCompletions> responseWithFunctionCallWithoutStream
         = await _client.GetChatCompletionsAsync(ChatCompletionOptions);

        ChatCompletions responseWithFunctionCall = responseWithFunctionCallWithoutStream.Value;

        return responseWithFunctionCall;
    }


}
