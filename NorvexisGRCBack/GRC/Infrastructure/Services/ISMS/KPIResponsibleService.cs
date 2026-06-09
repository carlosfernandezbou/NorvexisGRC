using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;
using GRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Services;

public class KPIResponsibleService : IKPIResponsibleService
{
    private readonly GrcDbContext _context;

    public KPIResponsibleService(GrcDbContext context)
    {
        _context = context;
    }

    public async Task<List<KPIResponsible>> GetAllAsync()
    {
        return await _context.KPIResponsibles.ToListAsync();
    }
    public async Task<KPIResponsible?> GetOneAsync(string id)
    {
        return await _context.KPIResponsibles
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<KPIResponsible> CreateAsync(string name)
    {
        var entity = new KPIResponsible
        {
            Id = Guid.NewGuid().ToString(),

            Responsible = name
        };

        _context.KPIResponsibles.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<KPIResponsible> CreateAsync(KPIResponsible kpiResponsible)
    {
        _context.KPIResponsibles.Add(kpiResponsible);
        await _context.SaveChangesAsync();

        return kpiResponsible;
    }

    public async Task<KPIResponsible?> DeleteAsync(string id)
    {
        var responsible = await _context.KPIResponsibles
            .FirstOrDefaultAsync(x => x.Id == id);

        if (responsible is null)
            return null;

        _context.KPIResponsibles.Remove(responsible);
        await _context.SaveChangesAsync();

        return responsible;
    }

    public async Task<KPIResponsible?> UpdateAsync(string id, string name)
    {
        var entity = await _context.KPIResponsibles
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null) return null;

        entity.Responsible = name;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<KPIResponsible> UpdateAsync(KPIResponsible kpiResponsible)
    {
        var existingResponsible = await _context.KPIResponsibles.FirstOrDefaultAsync(r => r.Id == kpiResponsible.Id);
        if (existingResponsible == null)
        {
            throw new Exception("KPI Responsible not found");
        }
        _context.Entry(existingResponsible).CurrentValues.SetValues(kpiResponsible);
        
        await _context.SaveChangesAsync();

        return kpiResponsible;
    }
}