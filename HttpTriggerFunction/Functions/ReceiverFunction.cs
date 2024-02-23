using HttpTriggerFunction.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HttpTriggerFunction.Functions;

public class ReceiverFunction(ILogger<ReceiverFunction> logger, IBlobStorageService blobStorageService)
{
    private readonly ILogger<ReceiverFunction> _logger = logger;
    private readonly IBlobStorageService _blobStorageService = blobStorageService;

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

            // Check if file is exists or not
            if (req.ContentType == null || req.Form == null || req.Form.Files.Count == 0)
            {
                return new BadRequestObjectResult("Please attach a PDF file in the request.");
            }

            // Get the pdf file from request body
            var file = req.Form.Files[0];

            // Check if the file is a PDF
            if (file.ContentType != "application/pdf")
            {
                return new BadRequestObjectResult($"Unsupported file type {file.ContentType}. Only Pdf file is Supported.");
            }

            // Upload the pdf file to the blob
            Uri blobUri = await _blobStorageService.UploadPdfIntoBlobContainer(name, file);

            return new OkObjectResult($"File uploaded successfully. Blob URI: {blobUri}");
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }
}
