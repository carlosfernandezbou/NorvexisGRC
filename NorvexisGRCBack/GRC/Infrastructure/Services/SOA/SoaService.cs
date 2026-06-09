using GRC.Core.Interfaces.SOA;
using GRC.Core.Models.SOA;
using GRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GRC.Infrastructure.Services;

public class SoaService : ISOAService
{
    private readonly GrcDbContext _context;

    public SoaService(GrcDbContext context)
    {
        _context = context;
    }

    public async Task<List<SOA>> GetAllAsync()
    {
        return await _context.SOAs.ToListAsync();
    }

    public async Task<SOA?> GetOneAsync(string id)
    {
        return await _context.SOAs.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<SOA> CreateAsync(string section, string control, string tittle, string objective, bool applicable, bool implemented, string justification, string evidence)
    {
        var soa = new SOA
        {
            Section = section,
            Control = control,
            Tittle = tittle,
            Objective = objective,
            Applicable = applicable,
            Implemented = implemented,
            Justification = justification,
            Evidence = evidence
        };

        _context.SOAs.Add(soa);
        await _context.SaveChangesAsync();

        return soa;
    }

    public async Task<SOA> CreateAsync(SOA soa)
    {
        _context.SOAs.Add(soa);
        await _context.SaveChangesAsync();

        return soa;
    }

    public async Task<SOA?> UpdateAsync(string id, string section, string control, string tittle, string objective, bool applicable, bool implemented, string justification, string evidence)
    {
        var soa = await _context.SOAs.FirstOrDefaultAsync(s => s.Id == id);

        if (soa == null)
        {
            return null;
        }

        soa.Section = section;
        soa.Control = control;
        soa.Tittle = tittle;
        soa.Objective = objective;
        soa.Applicable = applicable;
        soa.Implemented = implemented;
        soa.Justification = justification;
        soa.Evidence = evidence;

        await _context.SaveChangesAsync();

        return soa;
    }

    public async Task<SOA> UpdateAsync(SOA soa)
    {
        var existingSoa = await _context.SOAs.FirstOrDefaultAsync(s => s.Id == soa.Id);
        if (existingSoa == null)
        {
            throw new Exception("SOA not found");
        }

        _context.Entry(existingSoa).CurrentValues.SetValues(soa);
        await _context.SaveChangesAsync();

        return soa;
    }

    public async Task<SOA?> DeleteAsync(string id)
    {
        var soa = await _context.SOAs.FirstOrDefaultAsync(s => s.Id == id);

        if (soa == null)
        {
            return null;
        }

        _context.SOAs.Remove(soa);
        await _context.SaveChangesAsync();

        return soa;
    }

    public async Task<int> ImportFromJsonAsync(Stream jsonStream)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var rows = await JsonSerializer.DeserializeAsync<List<SOAImportDto>>(jsonStream, options);

        if (rows == null || rows.Count == 0)
            return 0;

        var soas = new List<SOA>();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Control))
                continue;

            var control = row.Control.Trim().Replace(",", ".");

            var applicable = string.Equals(row.Applicable?.Trim(), "true", StringComparison.OrdinalIgnoreCase)
                             || row.Applicable == "1";

            var implemented = string.Equals(row.Implemented?.Trim(), "true", StringComparison.OrdinalIgnoreCase)
                              || row.Implemented == "1";

            var soa = new SOA
            {
                Id = Guid.NewGuid().ToString(),
                Section = row.Section?.Trim() ?? string.Empty,
                Control = control,
                Tittle = row.Tittle?.Trim() ?? string.Empty,
                Objective = row.Objective?.Trim() ?? string.Empty,
                Applicable = applicable,
                Implemented = implemented,
                Justification = row.Justification?.Trim() ?? string.Empty,
                Evidence = row.Evidence?.Trim() ?? string.Empty
            };

            soas.Add(soa);
        }

        await _context.SOAs.AddRangeAsync(soas);
        await _context.SaveChangesAsync();

        return soas.Count;
    }
}