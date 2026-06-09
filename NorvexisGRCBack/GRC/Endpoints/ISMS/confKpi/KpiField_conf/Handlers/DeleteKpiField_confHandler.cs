using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Endpoints.ISMS.confKpi.Handlers;

public class DeleteKpiField_confHandler
{
    private readonly IKpiField_conf _kpiFieldService;
    private readonly ILogger<DeleteKpiField_confHandler> _logger;
    
    public DeleteKpiField_confHandler(
        IKpiField_conf kpiFieldService,
        ILogger<DeleteKpiField_confHandler> logger)
    {
        _kpiFieldService = kpiFieldService;
        _logger = logger;
    }

    public async Task<IResult> Handle(string id)
    {
        _logger.LogInformation("Deleting KPI Field {Id}", id);

        var kpiField = await _kpiFieldService.DeleteAsync(id);

        if (kpiField is null)
            return Results.NotFound(new { error = "KPI Field not found" });

        return Results.Ok(kpiField);
    }
}