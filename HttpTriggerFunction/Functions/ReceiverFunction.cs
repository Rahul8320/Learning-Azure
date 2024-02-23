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

            // Get the pdf file from request body
            Stream pdfStream = req.Body;

            // Check if file is exists or not
            if (pdfStream == null)
            {
                return new BadRequestObjectResult("Please attach a PDF file in the request body.");
            }

            // Upload the pdf file to the blob
            Uri blobUri = await _blobStorageService.UploadPdfIntoBlobContainer(name, pdfStream);

            return new OkObjectResult($"File uploaded successfully. Blob URI: {blobUri}");
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }
}
