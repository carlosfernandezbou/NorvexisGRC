using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;

namespace GRC.Endpoints.RiskManagement.Handlers;

public class UpdateRiskTreatmentHandler
{
    private readonly IRiskTreatmentService _service;

    public UpdateRiskTreatmentHandler(IRiskTreatmentService service)
    {
        _service = service;
    }

    public async Task<IResult> Handle(string id, UpdateRiskTreatmentRequest request)
    {
        // var item = await _service.UpdateAsync(
        //     id,
        //     request.RiskId,
        //     request.RiskTreatmentOption,
        //     request.TreatmentPlan,
        //     request.Owner,
        //     request.DueDate,
        //     request.Status,
        //     request.ResidualRisk,
        //     request.Comments
        // );

        RiskTreatment riskTreatment = new RiskTreatment
        {
            Id = id,
            RiskId = request.RiskId,
            RiskTreatmentOption = request.RiskTreatmentOption,
            TreatmentPlan = request.TreatmentPlan,
            Owner = request.Owner,
            DueDate = request.DueDate,
            Status = request.Status,
            ResidualRisk = request.ResidualRisk,
            Comments = request.Comments
        };

        riskTreatment = await _service.UpdateAsync(riskTreatment);

        if (riskTreatment == null)
        {
            return Results.NotFound(new { error = "RiskTreatment not found" });
        }

        return Results.Ok(riskTreatment);
    }
}