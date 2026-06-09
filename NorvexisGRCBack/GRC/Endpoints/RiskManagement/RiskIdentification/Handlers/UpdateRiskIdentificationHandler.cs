using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;

namespace GRC.Endpoints.RiskManagement.Handlers;

public class UpdateRiskIdentificationHandler
{
    private readonly IRiskIdentificationService _service;

    public UpdateRiskIdentificationHandler(IRiskIdentificationService service)
    {
        _service = service;
    }

    public async Task<IResult> Handle(string id, UpdateRiskIdentificationRequest request)
    {
        RiskIdentification riskIdentification = new RiskIdentification
        {
            Id = id,
            RiskId = request.RiskId,
            Category = request.Category,
            SubCategory = request.SubCategory,
            AssetType = request.AssetType,
            AssetName = request.AssetName,
            Vulnerability = request.Vulnerability,
            Threats = request.Threats,
            CoreValueImpacted = request.CoreValueImpacted
        };

        riskIdentification = await _service.UpdateAsync(riskIdentification);

        // var item = await _service.UpdateAsync(
        //             id,
        //             request.RiskId,
        //             request.Category,
        //             request.SubCategory,
        //             request.AssetType,
        //             request.AssetName,
        //             request.Vulnerability,
        //             request.Threats,
        //             request.CoreValueImpacted
        //         );

        if (riskIdentification == null)
        {
            return Results.NotFound(new { error = "RiskIdentification not found" });
        }

        return Results.Ok(riskIdentification);
    }
}