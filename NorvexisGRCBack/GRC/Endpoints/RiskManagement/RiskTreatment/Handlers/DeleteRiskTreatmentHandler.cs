using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.RiskManagement;

namespace GRC.Endpoints.RiskManagement.Handlers;

public class DeleteRiskTreatmentHandler
{
    private readonly IRiskTreatmentService _service;

    public DeleteRiskTreatmentHandler(IRiskTreatmentService service)
    {
        _service = service;
    }

    public async Task<IResult> Handle(string id)
    {
        var item = await _service.DeleteAsync(id);

        if (item == null)
        {
            return Results.NotFound(new { error = "RiskTreatment not found" });
        }

        return Results.Ok(item);
    }
}