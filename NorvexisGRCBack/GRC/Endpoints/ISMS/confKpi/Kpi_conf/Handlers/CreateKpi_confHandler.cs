using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Endpoints.ISMS.confKpi.Handlers;

public class CreateKpi_confHandler
{
    private readonly IKpi_conf _kpiService;
    private readonly ILogger<CreateKpi_confHandler> _logger;

    public CreateKpi_confHandler(
        IKpi_conf kpiService,
        ILogger<CreateKpi_confHandler> logger)
    {
        _kpiService = kpiService;
        _logger = logger;
    }

    public async Task<IResult> Handle(CreateKpi_confRequest request)
    {

        if (string.IsNullOrWhiteSpace(request.KpiTypeConfId))
            return Results.BadRequest(new { error = "KpiTypeConfId is required" });

        Kpi_conf kpi = new Kpi_conf
        {
            KpiTypeConfId = request.KpiTypeConfId
        };

        kpi = await _kpiService.CreateAsync(kpi);

        return Results.Ok(kpi);
    }
}