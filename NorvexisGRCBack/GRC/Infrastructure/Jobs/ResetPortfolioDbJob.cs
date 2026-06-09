using Microsoft.Azure.Cosmos;
using Quartz;
using GRC.Infrastructure.Data.Seed;

namespace GRC.Infrastructure.Jobs;

public class ResetPortfolioDbJob : IJob
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ResetPortfolioDbJob> _logger;

    public ResetPortfolioDbJob(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger<ResetPortfolioDbJob> logger)
    {
        _services = services;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var endpoint = _configuration["CosmosDb:AccountEndpoint"]!;
        var key = _configuration["CosmosDb:AccountKey"]!;
        var databaseName = _configuration["CosmosDb:DatabaseName"]!;

        try
        {
            var client = new CosmosClient(endpoint, key);

            _logger.LogInformation("Reset de BD iniciado");

            await client.GetDatabase(databaseName).DeleteAsync();

            await CosmosInitializer.InitializeAsync(_services, endpoint, key, databaseName);
            await UserSeeder.SeedAsync(_services);
            await KpiConfSeeder.SeedAsync(_services);
            await SoaSeeder.SeedAsync(_services);

            _logger.LogInformation("Reset de BD terminado correctamente");
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            await CosmosInitializer.InitializeAsync(_services, endpoint, key, databaseName);
            await UserSeeder.SeedAsync(_services);
            await KpiConfSeeder.SeedAsync(_services);
            await SoaSeeder.SeedAsync(_services);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reseteando la BD");
        }
    }
}