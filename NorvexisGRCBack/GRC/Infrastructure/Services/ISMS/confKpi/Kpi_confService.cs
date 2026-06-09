using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;
using GRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Services;

public class Kpi_confService : IKpi_conf
{
    private readonly GrcDbContext _context;

    public Kpi_confService(GrcDbContext context)
    {
        _context = context;
    }

    public async Task<List<Kpi_conf>> GetAllAsync()
    {
        return await _context.Kpi_confs.ToListAsync();
    }

    public async Task<Kpi_conf?> GetOneAsync(string id)
    {
        return await _context.Kpi_confs.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<Kpi_conf> CreateAsync(Kpi_conf kpi)
    {
        _context.Kpi_confs.Add(kpi);
        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<Kpi_conf> UpdateAsync(Kpi_conf kpi)
    {
        var existingKpi = await _context.Kpi_confs.FirstOrDefaultAsync(k => k.Id == kpi.Id);
        if (existingKpi == null)
        {
            throw new Exception("KPI not found");
        }

        _context.Entry(existingKpi).CurrentValues.SetValues(kpi);
        
        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<Kpi_conf?> DeleteAsync(string id)
    {
        var kpi = await _context.Kpi_confs.FirstOrDefaultAsync(k => k.Id == id);

        if (kpi == null)
        {
            return null;
        }

        _context.Kpi_confs.Remove(kpi);
        await _context.SaveChangesAsync();

        return kpi;
    }
}