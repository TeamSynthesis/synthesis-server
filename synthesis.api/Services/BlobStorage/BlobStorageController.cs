using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Synthesis.Api.Services.BlobStorage;

namespace synthesis.api.Services.BlobStorage;

[ApiController]
[Route("api/[controller]")]
public class BlobStorageController : ControllerBase
{
    private readonly R2CloudStorage _blobService;

    public BlobStorageController(R2CloudStorage blobService)
    {
        _blobService = blobService;
    }


    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, string fileName)
    {
        var fileExtension = Path.GetExtension(file.FileName);
        var response = await _blobService.UploadFileAsync(file, $"img_u_{fileName}{fileExtension}");
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{blobName}")]
    public async Task<IActionResult> DeleteBlob(string blobName)
    {
        return Ok();
    }
}

