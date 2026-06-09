using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;

namespace GRC.Endpoints.RiskManagement.Handlers;

public class UpdateRiskAssessmentHandler
{
    private readonly IRiskAssessmentService _service;

    public UpdateRiskAssessmentHandler(IRiskAssessmentService service)
    {
        _service = service;
    }

    public async Task<IResult> Handle(string id, UpdateRiskAssessmentRequest request)
    {

        RiskAssessment riskAssessment = new RiskAssessment
        {
            Id = id,
            RiskId = request.RiskId,
            RiskImpactDescription = request.RiskImpactDescription,
            ExistingControls = request.ExistingControls,
            Impact = request.Impact,
            Likelihood = request.Likelihood,
            RiskCategory = request.RiskCategory
        };

        riskAssessment = await _service.UpdateAsync(riskAssessment);

        // var item = await _service.UpdateAsync(
        //     id,
        //     request.RiskId,
        //     request.RiskImpactDescription,
        //     request.ExistingControls,
        //     request.Impact,
        //     request.Likelihood,
        //     request.RiskCategory
        // );

        if (riskAssessment == null)
        {
            return Results.NotFound(new { error = "RiskAssessment not found" });
        }

        return Results.Ok(riskAssessment);
    }
}