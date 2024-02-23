using Azure.Storage.Blobs;
using HttpTriggerFunction.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HttpTriggerFunction.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly string _connectionString;
    private readonly string _containerName;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(ILogger<BlobStorageService> logger)
    {
        _logger = logger;

        _connectionString = Environment.GetEnvironmentVariable(
            "BlobStorageConnectionString",
            EnvironmentVariableTarget.Process) ?? throw new Exception("Connection string can not be null!");

        _containerName = Environment.GetEnvironmentVariable(
            "BlobContainerName",
            EnvironmentVariableTarget.Process) ?? throw new Exception("Container name can not be null!");
    }

    private BlobContainerClient GetBlobContainerClient()
    {
        try
        {
            BlobServiceClient blobServiceClient = new(_connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            return blobContainerClient;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Uri> UploadPdfIntoBlobContainer(string name, IFormFile pdfFile)
    {
        try
        {
            BlobContainerClient blobContainerClient = GetBlobContainerClient();

            // Format the timestamp to include date and time
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            // create a unique name for the blob
            string blobName = $"{name}_{timestamp}.pdf";
            _logger.LogInformation($"Blob name: {blobName}");

            // Upload the pdf file to the blob
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            using (Stream pdfStream = pdfFile.OpenReadStream())
            {
                await blobClient.UploadAsync(pdfStream, true);
            }

            // Log the blob file uri
            _logger.LogInformation($"File uploaded to blob storage. Blob URL: {blobClient.Uri}");

            return blobClient.Uri;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
