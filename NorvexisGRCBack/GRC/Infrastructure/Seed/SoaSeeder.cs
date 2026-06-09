using System.Text.Json;
using GRC.Core.Models.SOA;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Data.Seed;

public static class SoaSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<GrcDbContext>();

        await SeedFromJson<SOA>(
            context,
            "SOAs.json",
            context.SOAs
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
