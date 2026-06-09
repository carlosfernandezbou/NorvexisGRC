namespace GRC.Infrastructure.Data.Seed;

using Microsoft.Azure.Cosmos;

public static class CosmosInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, string endpoint, string key, string databaseName)
    {
        using var scope = services.CreateScope();

        var grcContext = scope.ServiceProvider.GetRequiredService<GrcDbContext>();
        await grcContext.Database.EnsureCreatedAsync();

        var identityContext = scope.ServiceProvider.GetRequiredService<IdentityCosmosDbContext>();
        await identityContext.Database.EnsureCreatedAsync();

        var cosmosClient = new CosmosClient(endpoint, key);

        var properties = new ContainerProperties(
        id: "AuditEvents",
        partitionKeyPath: "/EventType")
        {
            DefaultTimeToLive = 60 * 60 * 24 * 365 // segundos * minutos * horas * dias
        };

        await cosmosClient
            .GetDatabase(databaseName)
            .CreateContainerIfNotExistsAsync(
                properties,
                throughput: 400);
    }
}