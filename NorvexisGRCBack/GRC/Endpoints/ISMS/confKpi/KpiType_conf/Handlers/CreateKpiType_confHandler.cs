using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Endpoints.ISMS.confKpi.Handlers;

public class CreateKpiType_confHandler
{
    private readonly IKpiType_conf _kpiTypeService;
    private readonly ILogger<CreateKpiType_confHandler> _logger;

    public CreateKpiType_confHandler(
        IKpiType_conf kpiTypeService,
        ILogger<CreateKpiType_confHandler> logger)
    {
        _kpiTypeService = kpiTypeService;
        _logger = logger;
    }

    public async Task<IResult> Handle(CreateKpiType_confRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
        {
            return Results.BadRequest(new { error = "KPI Type is required" });
        }

        _logger.LogInformation("Creating KPI Type {KpiType}", request.Type);

        KpiType_conf kpiType = new KpiType_conf
        {
            Type = request.Type
        };

        kpiType = await _kpiTypeService.CreateAsync(kpiType);


        return Results.Ok(kpiType);
    }
}