using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class CreateKPIResponsibleHandler
{
    private readonly IKPIResponsibleService _kpiResponsibleService;
    private readonly ILogger<CreateKPIResponsibleHandler> _logger;

    public CreateKPIResponsibleHandler(
        IKPIResponsibleService kpiResponsibleService,
        ILogger<CreateKPIResponsibleHandler> logger)
    {
        _kpiResponsibleService = kpiResponsibleService;
        _logger = logger;
    }

    public async Task<IResult> Handle(CreateKPIResponsibleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Responsible))
        {
            return Results.BadRequest(new { error = "Responsible is required" });
        }

        _logger.LogInformation("Creating KPI Responsible {Responsible}", request.Responsible);

        KPIResponsible kpiResponsible = new KPIResponsible
        {
            Responsible = request.Responsible
        };

        kpiResponsible = await _kpiResponsibleService.CreateAsync(kpiResponsible);

        // var responsible = await _kpiResponsibleService.CreateAsync(request.Responsible);

        return Results.Ok(kpiResponsible);
    }
}