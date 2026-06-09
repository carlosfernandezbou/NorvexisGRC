using Microsoft.Extensions.Configuration;

namespace GRC.Endpoints;

public static class Health
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health/config", (IConfiguration configuration) =>
        {
            var endpointKey = "CosmosDb:AccountEndpoint";
            var keyKey = "CosmosDb:AccountKey";
            var dbKey = "CosmosDb:DatabaseName";
            var sourceKey = "Source";

            var endpoint = configuration[endpointKey];
            var accountKey = configuration[keyKey];
            var databaseName = configuration[dbKey];
            var source = configuration[sourceKey];

            string? endpointProvider = null;
            string? keyProvider = null;
            string? dbProvider = null;
            string? sourceProvider = null;

            if (configuration is IConfigurationRoot root)
            {
                endpointProvider = FindProvider(root, endpointKey);
                keyProvider = FindProvider(root, keyKey);
                dbProvider = FindProvider(root, dbKey);
                sourceProvider = FindProvider(root, sourceKey);
            }

            return Results.Ok(new
            {
                message = "Configuración Cosmos detectada",
                values = new
                {
                    Source = source,
                    AccountEndpoint = endpoint,
                    AccountKey = string.IsNullOrWhiteSpace(accountKey) ? null : "***CONFIGURED***",
                    DatabaseName = databaseName
                },
                providers = new
                {
                    Source = sourceProvider,
                    AccountEndpoint = endpointProvider,
                    AccountKey = keyProvider,
                    DatabaseName = dbProvider
                },
                interpretation = new
                {
                    isUsingDockerComposeEnvironment =
                        endpointProvider?.Contains("EnvironmentVariablesConfigurationProvider") == true ||
                        keyProvider?.Contains("EnvironmentVariablesConfigurationProvider") == true ||
                        dbProvider?.Contains("EnvironmentVariablesConfigurationProvider") == true,

                    isUsingAppsettingsJson =
                        endpointProvider?.Contains("JsonConfigurationProvider") == true ||
                        keyProvider?.Contains("JsonConfigurationProvider") == true ||
                        dbProvider?.Contains("JsonConfigurationProvider") == true
                }
            });
        })
        .WithName("HealthConfig")
        .WithTags("Health");

        return app;
    }

    private static string? FindProvider(IConfigurationRoot root, string key)
    {
        foreach (var provider in root.Providers.Reverse())
        {
            if (provider.TryGet(key, out var _))
            {
                return provider.ToString();
            }
        }

        return null;
    }
}