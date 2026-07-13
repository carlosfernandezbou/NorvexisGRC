using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GRC.Infrastructure.Cosmos;

public static class CosmosTls
{
    public static bool DisableTlsValidation(IConfiguration configuration) =>
        configuration.GetValue<bool>("CosmosDb:DisableTlsValidation");


    public static void ConfigureClientOptions(CosmosClientOptions options)
    {
        options.ConnectionMode = ConnectionMode.Gateway;
        options.LimitToEndpoint = true;
        options.ServerCertificateCustomValidationCallback = (_, _, _) => true;
    }

    public static CosmosClientOptions CreateClientOptions()
    {
        var options = new CosmosClientOptions();
        ConfigureClientOptions(options);
        return options;
    }

    public static void ConfigureEfCore(CosmosDbContextOptionsBuilder cosmos)
    {
        cosmos.ConnectionMode(ConnectionMode.Gateway);
        cosmos.LimitToEndpoint(true);
        cosmos.HttpClientFactory(() => new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }));
    }
}
