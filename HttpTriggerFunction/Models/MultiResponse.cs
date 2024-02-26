using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace HttpTriggerFunction.Models;

public class MultiResponse
{
    [QueueOutput("out-queue", Connection = "AzureWebJobsStorage")]
    public string[] Messages { get; set; } = default!;
    public IActionResult Result { get; set; } = default!;
}
