using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Endpoints.ISMS.confKpi.Handlers;

public class UpdateKpiType_confHandler
{
    private readonly IKpiType_conf _kpiTypeService;
    private readonly ILogger<UpdateKpiType_confHandler> _logger;

    public UpdateKpiType_confHandler(
        IKpiType_conf kpiTypeService,
        ILogger<UpdateKpiType_confHandler> logger)
    {
        _kpiTypeService = kpiTypeService;
        _logger = logger;
    }

    public async Task<IResult> Handle(string id, UpdateKpiType_confRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
        {
            return Results.BadRequest(new { error = "KPI Type is required" });
        }

        _logger.LogInformation("Updating KPI Type {Id}", id);

        KpiType_conf kpiType = new KpiType_conf
        {
            Id = id,
            Type = request.Type,
        };

        kpiType = await _kpiTypeService.UpdateAsync(kpiType);

        // var kpiType = await _kpiTypeService.UpdateAsync(id, request.Type);

        if (kpiType is null)
            return Results.NotFound(new { error = "KPI Type not found" });

        return Results.Ok(kpiType);
    }
}