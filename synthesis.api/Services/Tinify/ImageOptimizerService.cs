using System.Text.Json.Nodes;
using RestSharp;
using RestSharp.Authenticators;

namespace synthesis.api.Services.Tinify;

public interface IImageOptimizerService
{
    Task<string> OptimizeImage(string url);
}
public class ImageOptimizerService:IImageOptimizerService
{
    private const  string _apiKey = "BqQGsm20wy1tXM2PGMrbTXXpSHwKnQRM";
    
    public ImageOptimizerService()
    {
        
    }
    public async Task<string> OptimizeImage(string url)
    {
        var options = new RestClientOptions
        {
            Authenticator = new HttpBasicAuthenticator("api", _apiKey),
            BaseUrl = new Uri("https://api.tinify.com/shrink")
        };

        var client = new RestClient(options);
        var request = new RestRequest();

        request.AddJsonBody(new
        {
            source = new
            {
               url = url  
            }
        });

        request.Method =Method.Post;

        var result = await client.ExecuteAsync(request);

        if (!result.IsSuccessful)
        {
            return url;
        }
        
        var resultUrl =(string) JsonObject.Parse(result.Content)["output"]["url"];
        
        return resultUrl;
    }
}
