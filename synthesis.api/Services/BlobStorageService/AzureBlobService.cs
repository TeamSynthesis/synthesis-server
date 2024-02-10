using System.Reflection.Metadata;
using Azure;
using Azure.Messaging;
using Azure.Storage.Blobs;
using synthesis.api.Mappings;

namespace synthesis.api.Services.BlobStorageService;

public class AzureBlobService
{
    private readonly BlobContainerClient _blobContainerClient;
    private readonly IConfiguration _configuration;
    private readonly string SasKey;

    private string Uri;
    public AzureBlobService(IConfiguration configuration)
    {
        _configuration = configuration;

        var cnnString = _configuration.GetSection("AzureBlobStorage")["ConnectionString"];
        var container = _configuration.GetSection("AzureBlobStorage")["Container"];
        var storageAccount = _configuration.GetSection("AzureBlobStorage")["StorageAccount"];
        SasKey = _configuration.GetSection("AzureBlobStorage")["SasKey"];


        Uri = $"https://{storageAccount}.blob.core.windows.net";

        var blobServiceClient = new BlobServiceClient(cnnString);

        _blobContainerClient = blobServiceClient.GetBlobContainerClient(container);
    }

    public async Task<List<BlobDto>> GetAllBlobs()
    {
        List<BlobDto> blobs = new List<BlobDto>();

        await foreach (var blob in _blobContainerClient.GetBlobsAsync())
        {
            var uri = _blobContainerClient.Uri.ToString();
            var name = blob.Name;
            var fullUri = $"{Uri}/{name}?{SasKey}";

            blobs.Add(new BlobDto { Uri = fullUri, Name = name });
        }

        return blobs;
    }

    public async Task<GlobalResponse<BlobDto>> Upload(IFormFile blob)
    {


        var client = _blobContainerClient.GetBlobClient(blob.FileName);


        await using (Stream? data = blob.OpenReadStream())
        {
            await client.UploadAsync(data);
        }

        var blobToReturn = new BlobDto() { Uri = client.Uri.AbsoluteUri + "?" + SasKey, Name = client.Name };

        return new GlobalResponse<BlobDto>(true, "upload success", value: blobToReturn);

    }

    public async Task<GlobalResponse<BlobDto>> Delete(string blobFileName)
    {
        var blob = _blobContainerClient.GetBlobClient(blobFileName);

        await blob.DeleteAsync();

        return new GlobalResponse<BlobDto>(true, "delete blob success");
    }
}

