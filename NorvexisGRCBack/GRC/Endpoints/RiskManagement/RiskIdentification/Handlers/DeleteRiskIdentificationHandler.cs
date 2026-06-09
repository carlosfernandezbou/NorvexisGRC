using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.RiskManagement;

namespace GRC.Endpoints.RiskManagement.Handlers;

public class DeleteRiskIdentificationHandler
{
    private readonly IRiskIdentificationService _service;

    public DeleteRiskIdentificationHandler(IRiskIdentificationService service)
    {
        _service = service;
    }

    public async Task<IResult> Handle(string id)
    {
        var item = await _service.DeleteAsync(id);

        if (item == null)
        {
            return Results.NotFound(new { error = "RiskIdentification not found" });
        }

        return Results.Ok(item);
    }
}