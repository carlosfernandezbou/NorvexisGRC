using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Endpoints.ISMS.confKpi.Handlers;

public class UpdateKpiField_confHandler
{
    private readonly IKpiField_conf _kpiFieldService;
    private readonly ILogger<UpdateKpiField_confHandler> _logger;

    public UpdateKpiField_confHandler(
        IKpiField_conf kpiFieldService,
        ILogger<UpdateKpiField_confHandler> logger)
    {
        _kpiFieldService = kpiFieldService;
        _logger = logger;
    }

    public async Task<IResult> Handle(string id, UpdateKpiField_confRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FieldName))
        {
            return Results.BadRequest(new { error = "KPI Field Name is required" });
        }

        _logger.LogInformation("Updating KPI Field {Id}", id);

        KpiField_conf kpiField = new KpiField_conf
        {
            Id = id,
            FieldName = request.FieldName,
            DefaultValue = request.DefaultValue
        };

        kpiField = await _kpiFieldService.UpdateAsync(kpiField);

        if (kpiField is null)
            return Results.NotFound(new { error = "KPI Field not found" });

        return Results.Ok(kpiField);
    }
}