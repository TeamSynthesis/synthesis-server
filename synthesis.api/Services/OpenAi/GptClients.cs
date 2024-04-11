using Azure;
using Azure.AI.OpenAI;

namespace synthesis.api.Services.OpenAi;


public static class GptClients
{
    /*
     * France Central: gpt4FC - 180 rpm
     *                 GPT3FC - 6 rpm
     * EastUs: GPT3Turbo - 6 rpm
     *         DALLE - 2 CU rpm
     * Norway East: GPT 4 - 6rpm
     */
    
    private static readonly string _OpenAiGPT4Endpoint = "https://synthesis-gpt4.openai.azure.com/";
    private static readonly string _OpenAiGPT4Key = "fb499762f958431e85c2f12c3c871e17";
    public static readonly string _GPT4Deployment = "synthesis-gpt4";
    public static OpenAIClient GPT4()
    {
        return new OpenAIClient(new Uri(_OpenAiGPT4Endpoint), new AzureKeyCredential(_OpenAiGPT4Key));
    }
    
    private static readonly string _OpenAiGPT4FCEndpoint = "https://synthesis-aiv2.openai.azure.com/";
    private static readonly string _OpenAiGPT4FCKey = "0c98227d5bf94f06a6711946a75ea53e";
    public static readonly string _GPT4FCDeployment = "gpt4FC";
    public static OpenAIClient GPT4FC()
    {
        return new OpenAIClient(new Uri(_OpenAiGPT4FCEndpoint), new AzureKeyCredential(_OpenAiGPT4FCKey));
    }

    
    private static readonly string _OpenAiGPT3Endpoint = "https://synthesis-aiv2.openai.azure.com/";
    private static readonly string _OpenAiGPT3Key = "0c98227d5bf94f06a6711946a75ea53e";
    public static readonly string _GPT3TurboDeployment = "GPT3FC";
    public static OpenAIClient GPT3FC()
    {
        return new OpenAIClient(new Uri(_OpenAiGPT3FCEndpoint), new AzureKeyCredential(_OpenAiGPT3FCKey));
    }


    private static readonly string _OpenAiGPT3FCEndpoint = "https://synthesis-gpt.openai.azure.com/";
    private static readonly string _OpenAiGPT3FCKey = "49d211e2c9f9429a84d4a5ba82096e6c";
    public static readonly string _GPT3FCTurboDeployment = "gpt-3-16K";
    public static OpenAIClient GPT3Turbo()
    {
        return new OpenAIClient(new Uri(_OpenAiGPT3Endpoint), new AzureKeyCredential(_OpenAiGPT3Key));
    }

    
    private static readonly string _OpenAiDalleEndpoint = "https://synthesis-gpt.openai.azure.com/";
    private static readonly string _OpenAiDalleKey = "49d211e2c9f9429a84d4a5ba82096e6c";
    public static readonly string _DalleDeployment = "Dalle3";
    public static OpenAIClient Dalle()
    {
        return new OpenAIClient(new Uri(_OpenAiDalleEndpoint), new AzureKeyCredential(_OpenAiDalleKey));
    }


}