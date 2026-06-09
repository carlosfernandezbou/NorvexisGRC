using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class CreateKPIHandler
{
    private readonly IKPIService _kpiService;
    private readonly ILogger<CreateKPIHandler> _logger;
    public CreateKPIHandler(IKPIService kpiService, ILogger<CreateKPIHandler> logger)
    {
        _kpiService = kpiService;
        _logger = logger;
    }
    
    public async Task<IResult> Handle(CreateKPIRequest request)
    {

        if (string.IsNullOrWhiteSpace(request.KpiName))
        {
            return Results.BadRequest(new { error = "KpiName is required" });
        }

        _logger.LogInformation("Creating KPI with title {KpiName}", request.KpiName);
        
        KPI kpi = new KPI
        {
            KpiConfId = request.KpiConfId,
            KPICategoryId = request.KPICategoryId,
            KPI_Name = request.KpiName,
            KPIResponsibleId = request.KPIResponsibleId,
            Value = request.Value,
            TargetValue = request.TargetValue,
            Comments = request.Comments,
            PeriodDate = request.PeriodDate
        };

        kpi = await _kpiService.CreateAsync(kpi);

        // var kpi = await _kpiService.CreateAsync(
        //     request.KPICategoryId,
        //     request.KpiName,
        //     request.KPIResponsibleId,
        //     request.Value,
        //     request.TargetValue,
        //     request.Comments
        // );

        return Results.Ok(kpi);
    }
}