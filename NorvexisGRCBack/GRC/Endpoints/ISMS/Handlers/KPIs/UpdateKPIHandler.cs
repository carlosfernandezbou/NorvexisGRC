using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class UpdateKPIHandler
{
    private readonly IKPIService _kpiService;
    public UpdateKPIHandler(IKPIService kpiService)
    {
        _kpiService = kpiService;
    }

    public async Task<IResult> Handle(string id, UpdateKPIRequest request)
    {
        var existingKpi = await _kpiService.GetOneAsync(id);

        if (existingKpi == null)
        {
            return Results.NotFound(new { error = "KPI not found" });
        }

        existingKpi.KPICategoryId = request.KPICategoryId;
        existingKpi.KPI_Name = request.KpiName;
        existingKpi.KPIResponsibleId = request.KPIResponsibleId;
        existingKpi.Value = request.Value;
        existingKpi.TargetValue = request.TargetValue;
        existingKpi.Comments = request.Comments;

        var kpi = await _kpiService.UpdateAsync(existingKpi);

        return Results.Ok(kpi);
    }
}