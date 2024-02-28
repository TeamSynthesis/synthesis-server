using Azure;
using Azure.AI.OpenAI;

namespace synthesis.api.Services.OpenAi;


public static class GptClients
{
    private static readonly string _OpenAiGPT4Endpoint = "https://synthesis-ai.openai.azure.com/";
    private static readonly string _OpenAiGPT4Key = "cb53dc167aee459292e6e4cff7573476";
    public static readonly string _GPT4Deployment = "synthesis-gpt4";

    public static OpenAIClient GPT4()
    {
        return new OpenAIClient(new Uri(_OpenAiGPT4Endpoint), new AzureKeyCredential(_OpenAiGPT4Key));
    }

    private static readonly string _OpenAiGPT3Endpoint = "https://synthesis-aigen.openai.azure.com/";
    private static readonly string _OpenAiGPT3Key = "abd00d16b6d14646a192411cc53d0054";
    public static readonly string _GPT3TurboDeployment = "synthesis-gpt3";

    public static OpenAIClient GPT3Turbo()
    {
        return new OpenAIClient(new Uri(_OpenAiGPT3Endpoint), new AzureKeyCredential(_OpenAiGPT3Key));
    }

    private static readonly string _OpenAiDalleEndpoint = "https://synthesis-aigen.openai.azure.com/";
    private static readonly string _OpenAiDalleKey = "abd00d16b6d14646a192411cc53d0054";
    public static readonly string _DalleDeployment = "synthesis-gpt4";

    public static OpenAIClient Dalle()
    {
        return new OpenAIClient(new Uri(_OpenAiDalleEndpoint), new AzureKeyCredential(_OpenAiDalleKey));
    }


}