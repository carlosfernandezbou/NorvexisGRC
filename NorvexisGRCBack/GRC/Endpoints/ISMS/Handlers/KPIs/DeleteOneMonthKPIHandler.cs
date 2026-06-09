using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class DeleteOneMonthKPIHandler
{
    private readonly IKPIService _kpiService;

    public DeleteOneMonthKPIHandler(IKPIService kpiService)
    {
        _kpiService = kpiService;
    }

    public async Task<IResult> Handle(DeleteOneMonthKPIRequest request)
    {
        var deleted = await _kpiService.DeleteByMonthAsync(request.Year, request.Month);

        return Results.Ok(new
        {
            deleted
        });
    }
}