using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HttpTriggerFunction.Functions;

public class ProcessFunction(ILogger<ProcessFunction> logger)
{
    private readonly ILogger<ProcessFunction> _logger = logger;

    [Function("ProcessFunction")]
    public async Task Run(
        [BlobTrigger("pdf-storage/{name}",
        Connection = "BlobStorageConnectionString")]
        Stream stream,
        string name)
    {
        using var blobStreamReader = new StreamReader(stream);
        var content = await blobStreamReader.ReadToEndAsync();

        _logger.LogInformation($"C# Blob trigger function Processed blob \n Name: {name}\n Content: {content}");
    }
}
