using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HttpTriggerFunction.Functions;

public class HttpTriggerFunction(ILogger<HttpTriggerFunction> logger)
{
    private readonly ILogger<HttpTriggerFunction> _logger = logger;

    [Function("HttpFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "walkthrough")] HttpRequestData req)
    {
        _logger.LogInformation("'HTTPFunction' function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            Name = "Azure Function",
            CurrentTime = DateTime.UtcNow
        });

        return response;
    }
}
