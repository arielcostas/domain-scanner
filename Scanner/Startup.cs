using Microsoft.Azure.Cosmos;
using Scanner.Models;

namespace Scanner;

public static class Startup
{
    public static async Task SetupCosmosDb(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("CosmosDb");
        CosmosClient cosmos = new(connectionString);

        await cosmos.CreateDatabaseIfNotExistsAsync(AppConstants.DatabaseName);

        await cosmos
            .GetDatabase(AppConstants.DatabaseName)
            .CreateContainerIfNotExistsAsync(AppConstants.ReportsContainer, $"/{nameof(Report.DomainName)}");
        
        await cosmos
            .GetDatabase(AppConstants.DatabaseName)
            .CreateContainerIfNotExistsAsync(AppConstants.ReportsLeaseContainer, "/id");

        builder.Services.AddSingleton(cosmos.GetDatabase(AppConstants.DatabaseName));
    }
}