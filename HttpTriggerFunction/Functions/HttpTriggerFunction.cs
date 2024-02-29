using System.Net;
using Microsoft.AspNetCore.Http;
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

    [Function("CurrentTime")]
    [BlobOutput("pdf-storage/{name}-output.txt")]
    public string CurrentTime([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "current-time-utc")] HttpRequest req)
    {
        _logger.LogInformation("'CurrentTime' function processed a request.");

        string name = req.Query["name"].ToString();

        return DateTime.UtcNow.ToString();
    }
}
