using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class DeleteKPIHandler
{
    private readonly IKPIService _kpiService;
    public DeleteKPIHandler(IKPIService kpiService)
    {
        _kpiService = kpiService;
    }
    
    public async Task<IResult> Handle(string id)
    {
        var kpi = await _kpiService.DeleteAsync(id);

        if (kpi == null)
        {
            return Results.NotFound(new { error = "kpi not found" });
        }
        
        return Results.Ok(kpi);
    }
}