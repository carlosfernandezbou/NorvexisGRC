using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;
using GRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Services;

public class KpiField_confService : IKpiField_conf
{
    private readonly GrcDbContext _context;

    public KpiField_confService(GrcDbContext context)
    {
        _context = context;
    }

    public async Task<List<KpiField_conf>> GetAllAsync()
    {
        return await _context.KpiField_confs.ToListAsync();
    }

    public async Task<KpiField_conf?> GetOneAsync(string id)
    {
        return await _context.KpiField_confs.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<KpiField_conf> CreateAsync(KpiField_conf kpi)
    {
        _context.KpiField_confs.Add(kpi);
        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<KpiField_conf> UpdateAsync(KpiField_conf kpi)
    {
        var existingKpi = await _context.KpiField_confs.FirstOrDefaultAsync(k => k.Id == kpi.Id);
        if (existingKpi == null)
        {
            throw new Exception("KPI not found");
        }

        _context.Entry(existingKpi).CurrentValues.SetValues(kpi);
        
        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<KpiField_conf?> DeleteAsync(string id)
    {
        var kpi = await _context.KpiField_confs.FirstOrDefaultAsync(k => k.Id == id);

        if (kpi == null)
        {
            return null;
        }

        _context.KpiField_confs.Remove(kpi);
        await _context.SaveChangesAsync();

        return kpi;
    }
}