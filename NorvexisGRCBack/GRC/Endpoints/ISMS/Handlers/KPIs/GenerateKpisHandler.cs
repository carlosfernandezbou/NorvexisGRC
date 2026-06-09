using GRC.Core.Interfaces.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class GenerateKpisHandler
{
    private readonly IKPIService _kpiService;

    public GenerateKpisHandler(IKPIService kpiService)
    {
        _kpiService = kpiService;
    }

    public async Task<IResult> Handle(GenerateKpisRequest request)
    {
        if (request.Month < 1 || request.Month > 12)
        {
            return Results.BadRequest(new { error = "Month must be between 1 and 12" });
        }

        if (request.Year < 2000)
        {
            return Results.BadRequest(new { error = "Year is not valid" });
        }

        var date = new DateTime(request.Year, request.Month, 1);

        await _kpiService.GenerateKpisForDateAsync(date);

        return Results.Ok(new
        {
            message = "KPIs generated successfully",
            year = request.Year,
            month = request.Month
        });
    }
}
