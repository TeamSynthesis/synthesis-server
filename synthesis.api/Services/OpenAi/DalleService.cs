using Azure.AI.OpenAI;
using synthesis.api.Services.Tinify;

namespace synthesis.api.Services.OpenAi;

public interface IDalleService
{
    Task<string> GenerateImage(string prompt);
}

public class DalleService : IDalleService
{

    private OpenAIClient _client;
    private readonly IImageOptimizerService _imageOptimizer;

    public DalleService(IImageOptimizerService imageOptimizer)
    {
        _imageOptimizer = imageOptimizer;
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
        var dalleUrl = response.Value.Data[0].Url.ToString();

        var optimizedUrl = await _imageOptimizer.OptimizeImage(dalleUrl);
        
        return optimizedUrl;
    }
}