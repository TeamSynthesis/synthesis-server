namespace synthesis.api.Services.OpenAi;

using Azure;
using Azure.AI.OpenAI;

public interface IChatGptService
{
    Task<ChatCompletions> GenerateProject(string prompt);
}
public class ChatGptService : IChatGptService
{

    private static readonly string _OpenAiEndpoint = "https://manasseh-gpt.openai.azure.com/";
    private static readonly string _OpenAiKey = "5f78e95d6fb946a8b7805822bade4cb4";
    private static readonly string _DeploymentName = "synthesis-gpt4";
    private OpenAIClient _client;

    public ChatGptService()
    {
        _client = new OpenAIClient(new Uri(_OpenAiEndpoint), new AzureKeyCredential(_OpenAiKey));
    }

    public async Task<ChatCompletions> GenerateProject(string prompt)
    {



        Response<ChatCompletions> responseWithoutStream = await _client.GetChatCompletionsAsync(
        _DeploymentName,
        new ChatCompletionsOptions()

        {

            Messages =
            {
                new ChatMessage(ChatRole.System, GptSystemMessage.SoftwareProjectAssistant ), new ChatMessage(ChatRole.User, prompt),
            },
            Temperature = (float)0.7,
            MaxTokens = 4009,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        });


        ChatCompletions response = responseWithoutStream.Value;

        return response;

    }
}
