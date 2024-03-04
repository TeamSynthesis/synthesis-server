using Azure;
using Azure.AI.OpenAI;

namespace synthesis.api.Services.OpenAi;


public static class GptClients
{
    private static readonly string _OpenAiGPT4Endpoint = "https://synthesis-gpt4.openai.azure.com/";
    private static readonly string _OpenAiGPT4Key = "fb499762f958431e85c2f12c3c871e17";
    public static readonly string _GPT4Deployment = "synthesis-gpt4";

    public static OpenAIClient GPT4()
    {
        return new OpenAIClient(new Uri(_OpenAiGPT4Endpoint), new AzureKeyCredential(_OpenAiGPT4Key));
    }

    private static readonly string _OpenAiGPT3Endpoint = "https://synthesis-gpt.openai.azure.com/";
    private static readonly string _OpenAiGPT3Key = "49d211e2c9f9429a84d4a5ba82096e6c";
    public static readonly string _GPT3TurboDeployment = "gpt-3-16K";

    public static OpenAIClient GPT3Turbo()
    {
        return new OpenAIClient(new Uri(_OpenAiGPT3Endpoint), new AzureKeyCredential(_OpenAiGPT3Key));
    }

    private static readonly string _OpenAiDalleEndpoint = "https://synthesis-gpt4.openai.azure.com/";
    private static readonly string _OpenAiDalleKey = "fb499762f958431e85c2f12c3c871e17";
    public static readonly string _DalleDeployment = "synthesis-gpt4";

    public static OpenAIClient Dalle()
    {
        return new OpenAIClient(new Uri(_OpenAiDalleEndpoint), new AzureKeyCredential(_OpenAiDalleKey));
    }


}