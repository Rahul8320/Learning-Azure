using System.Text.Json;
using HttpTriggerFunction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HttpTrigger.Function
{
    public class HttpTriggerFunction
    {
        private readonly ILogger<HttpTriggerFunction> _logger;

        public HttpTriggerFunction(ILogger<HttpTriggerFunction> logger)
        {
            _logger = logger;
        }

        [Function("HttpTriggerFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string? name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (!string.IsNullOrEmpty(requestBody))
            {
                var data = JsonSerializer.Deserialize<NameRequestModel>(requestBody);

                name = string.IsNullOrEmpty(data?.Name) ? name : data?.Name;
            }


            if (string.IsNullOrEmpty(name))
            {
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

            return new OkObjectResult($"Hello, {name}");
        }
    }
}
