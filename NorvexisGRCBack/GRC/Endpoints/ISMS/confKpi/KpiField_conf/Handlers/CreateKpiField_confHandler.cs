using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Endpoints.ISMS.confKpi.Handlers;

public class CreateKpiField_confHandler
{
    private readonly IKpiField_conf _kpiFieldService;
    private readonly ILogger<CreateKpiField_confHandler> _logger;

    public CreateKpiField_confHandler(
        IKpiField_conf kpiFieldService,
        ILogger<CreateKpiField_confHandler> logger)
    {
        _kpiFieldService = kpiFieldService;
        _logger = logger;
    }

    public async Task<IResult> Handle(CreateKpiField_confRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.KpiConfId))
        {
            return Results.BadRequest(new { error = "KPI Type is required" });
        }

        _logger.LogInformation("Creating KPI Field {KpiField}", request.KpiConfId);

        KpiField_conf kpiField = new KpiField_conf
        {
            KpiConfId = request.KpiConfId,
            FieldName = request.FieldName,
            DefaultValue = request.DefaultValue
        };

        kpiField = await _kpiFieldService.CreateAsync(kpiField);


        return Results.Ok(kpiField);
    }
}