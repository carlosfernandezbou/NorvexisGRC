using System.Diagnostics.Metrics;

namespace GRC.Infrastructure.Metrics;

public static class AppMetrics
{
    public static readonly Meter Meter = new("GRC.Business");

    public static readonly Counter<int> KpiJobsExecuted =
        Meter.CreateCounter<int>("kpi_jobs_executed_total");

    public static readonly Counter<int> KpiJobsFailed =
        Meter.CreateCounter<int>("kpi_jobs_failed_total");
}