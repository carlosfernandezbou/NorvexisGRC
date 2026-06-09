using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;
using GRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Services;

public class KpiType_confService : IKpiType_conf
{
    private readonly GrcDbContext _context;

    public KpiType_confService(GrcDbContext context)
    {
        _context = context;
    }

    public async Task<List<KpiType_conf>> GetAllAsync()
    {
        return await _context.KpiType_confs.ToListAsync();
    }

    public async Task<KpiType_conf?> GetOneAsync(string id)
    {
        return await _context.KpiType_confs.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<KpiType_conf> CreateAsync(string kpiId, string type)
    {
        var kpi = new KpiType_conf
        {
            Type = type,
        };
    
        _context.KpiType_confs.Add(kpi);
        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<KpiType_conf> CreateAsync(KpiType_conf kpi)
    {
        _context.KpiType_confs.Add(kpi);
        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<KpiType_conf?> UpdateAsync(string id, string kpiId, string type)
    {
        var kpi = await _context.KpiType_confs.FirstOrDefaultAsync(k => k.Id == id);

        if (kpi == null)
        {
            return null;
        }

        kpi.Type = type;

        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<KpiType_conf> UpdateAsync(KpiType_conf kpi)
    {
        var existingKpi = await _context.KpiType_confs.FirstOrDefaultAsync(k => k.Id == kpi.Id);
        if (existingKpi == null)
        {
            throw new Exception("KPI not found");
        }

        _context.Entry(existingKpi).CurrentValues.SetValues(kpi);
        
        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<KpiType_conf?> DeleteAsync(string id)
    {
        var kpi = await _context.KpiType_confs.FirstOrDefaultAsync(k => k.Id == id);

        if (kpi == null)
        {
            return null;
        }

        _context.KpiType_confs.Remove(kpi);
        await _context.SaveChangesAsync();

        return kpi;
    }
}