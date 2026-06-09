using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Endpoints.ISMS.confKpi.Handlers;

public class DeleteKpiType_confHandler
{
    private readonly IKpiType_conf _kpiTypeService;
    private readonly ILogger<DeleteKpiType_confHandler> _logger;

    public DeleteKpiType_confHandler(
        IKpiType_conf kpiTypeService,
        ILogger<DeleteKpiType_confHandler> logger)
    {
        _kpiTypeService = kpiTypeService;
        _logger = logger;
    }

    public async Task<IResult> Handle(string id)
    {
        _logger.LogInformation("Deleting KPI Type {Id}", id);

        var kpiType = await _kpiTypeService.DeleteAsync(id);

        if (kpiType is null)
            return Results.NotFound(new { error = "KPI Type not found" });

        return Results.Ok(kpiType);
    }
}