using System.Text.Json;
using GRC.Core.Models.ISMS;
using GRC.Core.Models.ISMS.confKpi;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Data.Seed;

public static class KpiConfSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<GrcDbContext>();

        await SeedFromJson<KpiType_conf>(
            context,
            "KpiType_conf.json",
            context.KpiType_confs
        );

        await SeedFromJson<KPICategory>(
            context,
            "KpiCategories.json",
            context.KPICategories
        );

        await SeedFromJson<KPIResponsible>(
            context,
            "KpiResponsibles.json",
            context.KPIResponsibles
        );

        await SeedFromJson<Kpi_conf>(
            context,
            "Kpi_conf.json",
            context.Kpi_confs
        );

        await SeedFromJson<KpiField_conf>(
            context,
            "KpiField_conf.json",
            context.KpiField_confs
        );

        await context.SaveChangesAsync();
    }

    private static async Task SeedFromJson<TEntity>(
        GrcDbContext context,
        string fileName,
        DbSet<TEntity> dbSet
    ) where TEntity : class
    {
        var fullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Infrastructure",
            "Seed",
            "Json",
            fileName
        );

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Seed file not found: {fullPath}");

        var json = await File.ReadAllTextAsync(fullPath);

        var items = JsonSerializer.Deserialize<List<TEntity>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        if (items is null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            var idProperty = typeof(TEntity).GetProperty("Id");

            if (idProperty is null)
                throw new Exception($"{typeof(TEntity).Name} does not have an Id property");

            var id = idProperty.GetValue(item);

            var existingEntity = await dbSet
                .FirstOrDefaultAsync(x =>
                    EF.Property<object>(x, "Id")!.Equals(id)
                );

            if (existingEntity is null)
            {
                await dbSet.AddAsync(item);
            }
        }
    }
}
