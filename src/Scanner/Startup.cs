using Microsoft.Azure.Cosmos;
using Scanner.Models;

namespace Scanner;

public static class Startup
{
    public static async Task SetupCosmosDb(this WebApplicationBuilder builder)
    {
        var cosmosOptions = new CosmosOptions();
        builder.Configuration.GetSection(CosmosOptions.Cosmos).Bind(cosmosOptions);
        
        CosmosClient cosmos = new(
            cosmosOptions.Endpoint,
            cosmosOptions.Key
        );

        // Create the database and containers if they don't exist
        await cosmos.CreateDatabaseIfNotExistsAsync(AppConstants.DefaultDatabaseName);

        // The container for the reports
        await cosmos
            .GetDatabase(cosmosOptions.DatabaseName)
            .CreateContainerIfNotExistsAsync(AppConstants.ReportsContainer, $"/{nameof(Report.DomainName)}");

        // The lease container is used by the Change Feed Processor to keep track of the changes it has processed.
        await cosmos
            .GetDatabase(cosmosOptions.DatabaseName)
            .CreateContainerIfNotExistsAsync(AppConstants.ReportsLeaseContainer, "/id");

        // Register the Database with the DI container. I don't expose the entire CosmosClient to prevent using it to access other databases. 
        builder.Services.AddSingleton(cosmos.GetDatabase(AppConstants.DefaultDatabaseName));
    }
}

public class CosmosOptions
{
    public const string Cosmos = "Cosmos";

    public string Endpoint { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = AppConstants.DefaultDatabaseName;
}