using Azure;
using Azure.AI.OpenAI;

namespace synthesis.api.Services.OpenAi;

public interface IDalleService
{
    Task<string> GenerateImage(string prompt);
}

public class DalleService : IDalleService
{

    private OpenAIClient _client;


    public DalleService()
    {
        _client = GptClients.Dalle();
    }

    public async Task<string> GenerateImage(string prompt)
    {
        var options = new ImageGenerationOptions()
        {
            Prompt = prompt,
            DeploymentName = GptClients._DalleDeployment,
            Size = ImageSize.Size1024x1024,
            Style = ImageGenerationStyle.Vivid
        };

        var response = await _client.GetImageGenerationsAsync(options);

        return response.Value.Data[0].Url.ToString();
    }
}