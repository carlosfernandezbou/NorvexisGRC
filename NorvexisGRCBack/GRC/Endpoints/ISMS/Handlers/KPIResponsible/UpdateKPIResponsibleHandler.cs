using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class UpdateKPIResponsibleHandler
{
    private readonly IKPIResponsibleService _kpiResponsibleService;
    private readonly ILogger<UpdateKPIResponsibleHandler> _logger;

    public UpdateKPIResponsibleHandler(
        IKPIResponsibleService kpiResponsibleService,
        ILogger<UpdateKPIResponsibleHandler> logger)
    {
        _kpiResponsibleService = kpiResponsibleService;
        _logger = logger;
    }

    public async Task<IResult> Handle(string id, UpdateKPIResponsibleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Responsible))
        {
            return Results.BadRequest(new { error = "Responsible is required" });
        }

        _logger.LogInformation("Updating KPI Responsible {Id}", id);

        KPIResponsible responsible = new KPIResponsible
        {
            Id = id,
            Responsible = request.Responsible,

        };

        responsible = await _kpiResponsibleService.UpdateAsync(responsible);

        // var responsible = await _kpiResponsibleService.UpdateAsync(id, request.Responsible);

        if (responsible is null)
            return Results.NotFound(new { error = "KPI Responsible not found" });

        return Results.Ok(responsible);
    }
}