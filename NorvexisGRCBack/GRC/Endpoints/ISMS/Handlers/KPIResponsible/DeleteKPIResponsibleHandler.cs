using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class DeleteKPIResponsibleHandler
{
    private readonly IKPIResponsibleService _kpiResponsibleService;
    private readonly ILogger<DeleteKPIResponsibleHandler> _logger;

    public DeleteKPIResponsibleHandler(
        IKPIResponsibleService kpiResponsibleService,
        ILogger<DeleteKPIResponsibleHandler> logger)
    {
        _kpiResponsibleService = kpiResponsibleService;
        _logger = logger;
    }

    public async Task<IResult> Handle(string id)
    {
        _logger.LogInformation("Deleting KPI Responsible {Id}", id);

        var responsible = await _kpiResponsibleService.DeleteAsync(id);

        if (responsible is null)
            return Results.NotFound(new { error = "KPI Responsible not found" });

        return Results.Ok(responsible);
    }
}