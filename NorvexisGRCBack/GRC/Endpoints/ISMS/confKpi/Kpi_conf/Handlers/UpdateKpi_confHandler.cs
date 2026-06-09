using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Endpoints.ISMS.confKpi.Handlers;

public class UpdateKpi_confHandler
{
    private readonly IKpi_conf _kpiService;
    private readonly ILogger<UpdateKpi_confHandler> _logger;

    public UpdateKpi_confHandler(
        IKpi_conf kpiService,
        ILogger<UpdateKpi_confHandler> logger)
    {
        _kpiService = kpiService;
        _logger = logger;
    }

    public async Task<IResult> Handle(string id, UpdateKpi_confRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.KpiTypeConfId))
        {
            return Results.BadRequest(new { error = "KPI Name is required" });
        }

        _logger.LogInformation("Updating KPI {Id}", id);

        Kpi_conf kpi = new Kpi_conf
        {
            Id = id,
            KpiTypeConfId = request.KpiTypeConfId
        };

        kpi = await _kpiService.UpdateAsync(kpi);

        if (kpi is null)
            return Results.NotFound(new { error = "KPI not found" });

        return Results.Ok(kpi);
    }
}