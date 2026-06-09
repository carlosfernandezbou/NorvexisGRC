using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.RiskManagement;

namespace GRC.Endpoints.RiskManagement.Handlers;

public class DeleteRiskAssessmentHandler
{
    private readonly IRiskAssessmentService _service;

    public DeleteRiskAssessmentHandler(IRiskAssessmentService service)
    {
        _service = service;
    }

    public async Task<IResult> Handle(string id)
    {
        var item = await _service.DeleteAsync(id);

        if (item == null)
        {
            return Results.NotFound(new { error = "RiskAssessment not found" });
        }

        return Results.Ok(item);
    }
}