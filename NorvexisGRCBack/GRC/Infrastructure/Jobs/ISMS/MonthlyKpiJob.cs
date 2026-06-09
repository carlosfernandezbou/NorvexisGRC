using GRC.Core.Interfaces.ISMS;
using Quartz;
using GRC.Infrastructure.Metrics;
using Audit.Core;
 
namespace GRC.Infrastructure.Jobs;
 
public class MonthlyKpiJob : IJob
{
    private readonly IKPIService _kpiService;
 
    public MonthlyKpiJob(IKPIService kpiService)
    {
        _kpiService = kpiService;
    }
 
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            Console.WriteLine("MonthlyKpiJob ejecutado");
            await _kpiService.GenerateKpisAsync();
            Console.WriteLine("MonthlyKpiJob terminado correctamente");
            AppMetrics.KpiJobsExecuted.Add(1);
 
            await using (var audit = await AuditScope.CreateAsync("CronJobs:MonthlyKpi", () => new
            {
                JobName = "MonthlyKpiJob",
                Action = "GenerateKpis",
                Success = true,
                TimestampUtc = DateTime.UtcNow
            })){}
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR EN MonthlyKpiJob:");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException?.Message);
            AppMetrics.KpiJobsFailed.Add(1);
 
            await using (var audit = await AuditScope.CreateAsync("CronJobs:FailedMonthlyKpi", () => new
            {
                JobName = "MonthlyKpiJob",
                Action = "GenerateKpis",
                Success = false,
                ErrorMessage = ex.Message,
                TimestampUtc = DateTime.UtcNow
            })){}
        }
    }
}