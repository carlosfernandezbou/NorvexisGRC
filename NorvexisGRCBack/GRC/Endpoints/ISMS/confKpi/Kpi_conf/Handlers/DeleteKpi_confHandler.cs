using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Endpoints.ISMS.confKpi.Handlers;

public class DeleteKpi_confHandler
{
    private readonly IKpi_conf _kpiService;
    private readonly ILogger<DeleteKpi_confHandler> _logger;

    public DeleteKpi_confHandler(
        IKpi_conf kpiService,
        ILogger<DeleteKpi_confHandler> logger)
    {
        _kpiService = kpiService;
        _logger = logger;
    }

    public async Task<IResult> Handle(string id)
    {
        _logger.LogInformation("Deleting KPI {Id}", id);

        var kpi = await _kpiService.DeleteAsync(id);

        if (kpi is null)
            return Results.NotFound(new { error = "KPI not found" });

        return Results.Ok(kpi);
    }
}