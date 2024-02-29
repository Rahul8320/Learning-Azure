using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttpTriggerFunction.Functions;

public class CosmosDbTriggerFunction(ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<CosmosDbTriggerFunction>();
    private readonly string _cosmosDbEndpoint = Environment.GetEnvironmentVariable("CosmosDbEndpoint")!;
    private readonly string _cosmosDbKey = Environment.GetEnvironmentVariable("CosmosDbKey")!;
    private readonly string _cosmosDbDatabaseName = Environment.GetEnvironmentVariable("DatabaseName")!;
    private readonly string _cosmosDbContainerName = Environment.GetEnvironmentVariable("ContainerName")!;

    private Container GetContainer()
    {
        var cosmosClient = new CosmosClient(_cosmosDbEndpoint, _cosmosDbKey);
        var database = cosmosClient.GetDatabase(_cosmosDbDatabaseName);
        var container = database.GetContainer(_cosmosDbContainerName);

        return container;
    }

    [Function("CosmosDbTrigger")]
    public void Run([CosmosDBTrigger(
        databaseName: "TestDB",
        containerName: "TodoItems",
        Connection = "CosmosDBConnection",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<ToDoItem> todoItems)
    {
        if (todoItems != null && todoItems.Count > 0)
        {
            foreach (var doc in todoItems)
            {
                _logger.LogInformation("ToDoItem: \n Id: {id} \n Desc: {desc}", doc.Id, doc.Description);
            }
        }
        else
        {
            _logger.LogInformation("No Todo Items Found!");
        }
    }

    [Function("CosmosDBFunction")]
    public async Task<IActionResult> CosmosDBFunction([HttpTrigger(AuthorizationLevel.Function, "post", Route = "add-todo")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("CosmosDBFunction processed a request");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var todoItem = new ToDoItem()
            {
                Id = Guid.NewGuid(),
                Description = data.description,
                LastUpdated = DateTime.UtcNow
            };

            // Add the document to Cosmos DB
            ItemResponse<ToDoItem> response = await GetContainer().CreateItemAsync(todoItem, partitionKey: new PartitionKey(todoItem.Id.ToString()));

            _logger.LogInformation("Document added to Cosmos DB with ID: {response}", response.Resource);

            return new OkObjectResult(response.Resource);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [Function("CosmosDbOutPutBinding")]
    [CosmosDBOutput("TestDB", "TodoItems", Connection = "CosmosDBConnection", CreateIfNotExists = true, PartitionKey = "/id")]
    public async Task<object> CosmosDbOutPutBinding([HttpTrigger(AuthorizationLevel.Function, "post", Route = "create-new-todo")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("CosmosDbOutPutBinding processed a request");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var todoItem = new ToDoItem()
            {
                Id = Guid.NewGuid(),
                Description = data.description,
                LastUpdated = DateTime.UtcNow,
            };
            var partitionKey = new PartitionKey(todoItem.Id.ToString());

            return new { todoItem, partitionKey };
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class ToDoItem
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public DateTime LastUpdated { get; set; }
}