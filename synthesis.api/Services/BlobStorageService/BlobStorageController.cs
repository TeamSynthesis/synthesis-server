using Microsoft.AspNetCore.Mvc;

namespace synthesis.api.Services.BlobStorageService;

[ApiController]
[Route("api/[controller]")]
public class BlobStorageController : ControllerBase
{
    private readonly AzureBlobService _blobService;

    public BlobStorageController(AzureBlobService blobService)
    {
        _blobService = blobService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _blobService.GetAllBlobs();
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var response = await _blobService.Upload(file);

        return Ok(response);
    }

    [HttpDelete("{blobName}")]
    public async Task<IActionResult> DeleteBlob(string blobName)
    {
        await _blobService.Delete(blobName);
        return Ok();
    }
}

