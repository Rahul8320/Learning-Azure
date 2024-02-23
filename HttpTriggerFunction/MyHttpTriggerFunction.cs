using System.Text.Json;
using HttpTriggerFunction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HttpTriggerFunction;

public class MyHttpTriggerFunction(ILogger<MyHttpTriggerFunction> logger)
{
    private readonly ILogger<MyHttpTriggerFunction> _logger = logger;

    [Function("MyHttpTriggerFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("My Custom HTTP trigger function processed a request.");

        string? name = req.Query["name"];

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        // Get environment variables.
        _logger.LogInformation($"FUNCTIONS_WORKER_RUNTIME: {System.Environment.GetEnvironmentVariable("FUNCTIONS_WORKER_RUNTIME", EnvironmentVariableTarget.Process)}");

        if (!string.IsNullOrEmpty(requestBody))
        {
            var data = JsonSerializer.Deserialize<NameRequestModel>(requestBody);

            name = string.IsNullOrEmpty(data?.Name) ? name : data?.Name;
        }


        if (string.IsNullOrEmpty(name))
        {
            return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        return new OkObjectResult($"Welcome back, {name}. Now you can get the access code.");
    }
}
