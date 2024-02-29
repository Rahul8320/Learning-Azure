using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HttpTriggerFunction.Functions;

public class CosmosDbTriggerFunction(ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<CosmosDbTriggerFunction>();

    [Function("CosmosDbTrigger")]
    public void Run([CosmosDBTrigger(
        databaseName: "TestDB",
        containerName: "TodoItems",
        Connection = "CosmosDBConnection",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<ToDoItem> todoItems)
    {
        if (todoItems is not null && todoItems.Any())
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
}

public class ToDoItem
{
    public string? Id { get; set; }
    public string? Description { get; set; }
}
