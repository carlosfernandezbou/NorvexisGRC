using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;
using GRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Services;

public class KpiService : IKPIService
{
    private readonly GrcDbContext _context;

    public KpiService(GrcDbContext context)
    {
        _context = context;
    }

    public async Task<List<KPI>> GetAllAsync()
    {
        return await _context.KPIs.ToListAsync();
    }

    public async Task<KPI?> GetOneAsync(string id)
    {
        return await _context.KPIs.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<KPI> CreateAsync(string kpiCategoryId, string kpiName, string kpiResponsibleId, int value, string comments)
    {
        var kpi = new KPI
        {
            KPICategoryId = kpiCategoryId,
            KPI_Name = kpiName,
            KPIResponsibleId = kpiResponsibleId,
            Value = value,
            Comments = comments,
            CreatedAt = DateTime.UtcNow
        };

        _context.KPIs.Add(kpi);
        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<KPI> CreateAsync(KPI kpi)
    {
        _context.KPIs.Add(kpi);
        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<KPI?> UpdateAsync(string id, string kpiCategoryId, string kpiName, string kpiResponsibleId, int value, string comments)
    {
        var kpi = await _context.KPIs.FirstOrDefaultAsync(k => k.Id == id);

        if (kpi == null)
        {
            return null;
        }

        kpi.KPICategoryId = kpiCategoryId;
        kpi.KPI_Name = kpiName;
        kpi.KPIResponsibleId = kpiResponsibleId;
        kpi.Value = value;
        kpi.Comments = comments;

        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<KPI> UpdateAsync(KPI kpi)
    {
        var existingKpi = await _context.KPIs.FirstOrDefaultAsync(k => k.Id == kpi.Id);
        if (existingKpi == null)
        {
            throw new Exception("KPI not found");
        }

        _context.Entry(existingKpi).CurrentValues.SetValues(kpi);

        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<KPI?> DeleteAsync(string id)
    {
        var kpi = await _context.KPIs.FirstOrDefaultAsync(k => k.Id == id);

        if (kpi == null)
        {
            return null;
        }

        _context.KPIs.Remove(kpi);
        await _context.SaveChangesAsync();

        return kpi;
    }

    public async Task<int> DeleteByMonthAsync(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);

        var kpis = await _context.KPIs
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt < endDate)
            .ToListAsync();

        if (!kpis.Any())
        {
            return 0;
        }

        _context.KPIs.RemoveRange(kpis);
        await _context.SaveChangesAsync();

        return kpis.Count;
    }

    public async Task GenerateKpisAsync()
    {
         var today = new DateTime(
        DateTime.UtcNow.Year,
        DateTime.UtcNow.Month,
        1
    );

        await GenerateKpisForDateAsync(today);
    }

    public async Task GenerateKpisForDateAsync(DateTime date)
    {
        Console.WriteLine("GenerateKpis iniciado");

        var today = date.Date;
        var configs = await _context.Kpi_confs.ToListAsync();
        var types = await _context.KpiType_confs.ToListAsync();

        foreach (var config in configs)
        {
            var type = types.FirstOrDefault(x => x.Id == config.KpiTypeConfId);

            if (type == null)
            {
                continue;
            }

            if (!ShouldGenerate(type.Type, today))
            {
                Console.WriteLine($"No toca generar config {config.Id} de tipo {type.Type} hoy {today:yyyy-MM-dd}");
                continue;
            }

            var periodKey = GetPeriodKey(type.Type, today);

            var existingKpis = await _context.KPIs
                .Where(x => x.KpiConfId == config.Id && x.PeriodKey == periodKey)
                .ToListAsync();

            if (existingKpis.Any())
            {
                Console.WriteLine($"KPI ya existe para config {config.Id} y periodo {periodKey}");
                continue;
            }

            var fields = await _context.KpiField_confs
                .Where(x => x.KpiConfId == config.Id)
                .ToListAsync();

            var kpi = new KPI
            {
                KpiConfId = config.Id,
                Value = null,
                Comments = string.Empty,
                PeriodDate = today,
                CreatedAt = today,
                PeriodKey = periodKey,
                ManuallyCreate = false
            };

            foreach (var field in fields)
            {
                switch (field.FieldName)
                {
                    case "KPI_Name":
                        kpi.KPI_Name = field.DefaultValue ?? string.Empty;
                        break;

                    case "KPICategoryId":
                        kpi.KPICategoryId = field.DefaultValue ?? string.Empty;
                        break;

                    case "KPIResponsibleId":
                        kpi.KPIResponsibleId = field.DefaultValue ?? string.Empty;
                        break;
                        
                    case "targetValue":
                        kpi.TargetValue = field.DefaultValue != null && int.TryParse(field.DefaultValue, out var target) ? target : null;
                        break;
                }
            }

            _context.KPIs.Add(kpi);
        }

        await _context.SaveChangesAsync();
    }

    private static bool ShouldGenerate(string type, DateTime today)
    {
        return type switch
        {
            "Weekly" => today.DayOfWeek == DayOfWeek.Monday,

            "Biweekly" => today.DayOfWeek == DayOfWeek.Monday &&
                          GetIsoWeek(today) % 2 == 0,

            "Monthly" => today.Day == 1,

            "Bimonthly" => today.Day == 1 &&
                           (today.Month == 2 || today.Month == 4 || today.Month == 6 ||
                            today.Month == 8 || today.Month == 10 || today.Month == 12),

            "Quarterly" => today.Day == 1 &&
                           (today.Month == 3 || today.Month == 6 ||
                            today.Month == 9 || today.Month == 12),

            "Biannual" => today.Day == 1 &&
                          (today.Month == 6 || today.Month == 12),

            "Annual" => today.Day == 1 &&
                        today.Month == 12,

            _ => false
        };
    }

    private static string GetPeriodKey(string type, DateTime date)
    {
        return type switch
        {
            "Weekly" => $"{date.Year}-W{GetIsoWeek(date):D2}",

            "Biweekly" => $"{date.Year}-BW{((GetIsoWeek(date) - 1) / 2) + 1:D2}",

            "Monthly" => $"{date.Year}-{date.Month:D2}",

            "Bimonthly" => date.Month switch
            {
                2 => $"{date.Year}-BM1",
                4 => $"{date.Year}-BM2",
                6 => $"{date.Year}-BM3",
                8 => $"{date.Year}-BM4",
                10 => $"{date.Year}-BM5",
                12 => $"{date.Year}-BM6",
                _ => $"{date.Year}-BMX"
            },

            "Quarterly" => date.Month switch
            {
                3 => $"{date.Year}-Q1",
                6 => $"{date.Year}-Q2",
                9 => $"{date.Year}-Q3",
                12 => $"{date.Year}-Q4",
                _ => $"{date.Year}-QX"
            },

            "Biannual" => date.Month switch
            {
                6 => $"{date.Year}-H1",
                12 => $"{date.Year}-H2",
                _ => $"{date.Year}-HX"
            },

            "Annual" => $"{date.Year}",

            _ => $"{date.Year}-{date.Month:D2}"
        };
    }

    private static int GetIsoWeek(DateTime date)
    {
        return System.Globalization.ISOWeek.GetWeekOfYear(date);
    }
}
