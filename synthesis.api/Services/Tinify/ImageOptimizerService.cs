using Microsoft.AspNetCore.Mvc;
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
            BaseUrl = new Uri("api.tinify.com")
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

        throw new NotImplementedException();
    }
}

[ApiController]
[Route("api/[controller]")]
public class ImageOptimizeController : ControllerBase
{
    private readonly IImageOptimizerService _service;
    public ImageOptimizeController(IImageOptimizerService service)
    {
        _service = service;
    }

    public async Task<IActionResult> OptimizeImage([FromBody] string url)
    {
        var result =await  _service.OptimizeImage(url);
        return Ok(result);
    }
}