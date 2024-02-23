using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HttpTriggerFunction.Functions;

public class ReceiverFunction(ILogger<ReceiverFunction> logger)
{
    private readonly ILogger<ReceiverFunction> _logger = logger;

    [Function("ReceiverFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Get the name from query parameter
            string? name = req.Query["name"];

            // Check if name is exists or not
            if (string.IsNullOrEmpty(name))
            {
                return new BadRequestObjectResult("Name query parameter is missing or value is null.");
            }

            // Get the pdf file from request body
            Stream pdfStream = req.Body;

            // Check if file is exists or not
            if (pdfStream == null)
            {
                return new BadRequestObjectResult("Please attach a PDF file in the request body.");
            }

            // Save the pdf file to Azure Blob Storage
            string connectionString = System.Environment.GetEnvironmentVariable("BlobStorageConnectionString", EnvironmentVariableTarget.Process)!;
            string containerName = System.Environment.GetEnvironmentVariable("BlobContainerName", EnvironmentVariableTarget.Process)!;

            BlobServiceClient blobServiceClient = new(connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // create a unique name for the blob
            string blobName = $"{name}-{Guid.NewGuid()}.pdf";

            // Upload the pdf file to the blob
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(pdfStream, true);

            _logger.LogInformation($"File uploaded to blob storage. Blob URL: {blobClient.Uri}");

            return new OkObjectResult($"File uploaded successfully. Blob URL: {blobClient.Uri}");
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult($"Exception: {ex}");
        }
    }
}
