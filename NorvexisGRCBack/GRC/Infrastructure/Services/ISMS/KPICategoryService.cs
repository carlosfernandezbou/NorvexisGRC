using Microsoft.EntityFrameworkCore;
using GRC.Infrastructure.Data;
using GRC.Core.Models.ISMS;
using GRC.Core.Interfaces.ISMS;

public class KPICategoryService : IKPICategoryService
{
    private readonly GrcDbContext _context;

    public KPICategoryService(GrcDbContext context)
    {
        _context = context;
    }

    public async Task<List<KPICategory>> GetAllAsync()
    {
        return await _context.KPICategories.ToListAsync();
    }

    public async Task<KPICategory?> GetOneAsync(string id)
    {
        return await _context.KPICategories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<KPICategory> CreateAsync(string name)
    {
        var entity = new KPICategory
        {
            Id = Guid.NewGuid().ToString(),
            KPI_Category = name
        };

        _context.KPICategories.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<KPICategory> CreateAsync(KPICategory kpiCategory)
    {
        _context.KPICategories.Add(kpiCategory);
        await _context.SaveChangesAsync();

        return kpiCategory;
    }

    public async Task<KPICategory?> DeleteAsync(string id)
    {
        var category = await _context.KPICategories
            .FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
            return null;

        _context.KPICategories.Remove(category);
        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<KPICategory?> UpdateAsync(string id, string name)
    {
        var entity = await _context.KPICategories
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null) return null;

        entity.KPI_Category = name;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<KPICategory> UpdateAsync(KPICategory kpiCategory)
    {
        var existingCategory = await _context.KPICategories.FirstOrDefaultAsync(c => c.Id == kpiCategory.Id);
        if (existingCategory == null)
        {
            throw new Exception("KPI Category not found");
        }

        _context.Entry(existingCategory).CurrentValues.SetValues(kpiCategory);
        await _context.SaveChangesAsync();

        return kpiCategory;
    }
}