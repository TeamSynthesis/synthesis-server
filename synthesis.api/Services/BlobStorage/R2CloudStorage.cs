using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using synthesis.api.Mappings;
using synthesis.api.Services.BlobStorage;

namespace Synthesis.Api.Services.BlobStorage;

public class R2CloudStorage
{
    private readonly string _accountId = "eba4a9f2b36d407e214d0ebb88b67526";
    private readonly string _accessKey = "d0869570b38bc2a18b68aa0b8a19e960";
    private readonly string _secretKey = "fd09bd87515cbab55104e7229e2623644903d0b816689666066056deb87230bb";
    private readonly string _publicAccessUrl = "https://pub-3138467ba63445cd8ff78e20a22ef173.r2.dev/";



    private readonly string _bucketName = "synthesis-bucket";
    private readonly AmazonS3Client _r2Client;

    public R2CloudStorage()
    {
        var credentials = new BasicAWSCredentials(_accessKey, _secretKey);

        _r2Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = $"https://eba4a9f2b36d407e214d0ebb88b67526.r2.cloudflarestorage.com"
        });

    }

    public async Task<GlobalResponse<BlobDto>> UploadFileAsync(IFormFile file, string fileName)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = file.OpenReadStream(),
            DisablePayloadSigning = true
        };


        var response = await _r2Client.PutObjectAsync(request);

        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK || response.HttpStatusCode == System.Net.HttpStatusCode.Accepted)
        {
            var blob = new BlobDto() { Url = _publicAccessUrl + fileName };
            return new GlobalResponse<BlobDto>(true, "blob upload success", value: blob);
        }

        return new GlobalResponse<BlobDto>(false, "blob upload failed", errors: [$"something went wrong blob upload failed"]);
    }

}

