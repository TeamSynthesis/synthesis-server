using Azure;
using Azure.AI.OpenAI;

namespace synthesis.api.Services.OpenAi;

public interface IDalleService
{
    Task<string> GetIcon(string prompt);
}

public class DalleService : IDalleService
{
    private static readonly string _OpenAiEndpoint = "https://synthesis-aigen.openai.azure.com/";
    private static readonly string _OpenAiKey = "abd00d16b6d14646a192411cc53d0054";
    private static readonly string _DeploymentName = "synthesis-gpt4";

    private OpenAIClient _client;


    public DalleService()
    {
        _client = new OpenAIClient(new Uri(_OpenAiEndpoint), new AzureKeyCredential(_OpenAiKey));
    }

    public async Task<string> GetIcon(string prompt)
    {
        var options = new ImageGenerationOptions()
        {
            Prompt = prompt,
            DeploymentName = "Dalle3",
            Size = ImageSize.Size1024x1024,
            Style = ImageGenerationStyle.Vivid
        };

        var response = await _client.GetImageGenerationsAsync(options);

        return response.Value.Data[0].Url.ToString();
    }
}