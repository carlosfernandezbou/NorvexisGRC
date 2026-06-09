using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;

namespace GRC.Endpoints.RiskManagement.Handlers;

public class CreateRiskAssessmentHandler
{
    private readonly IRiskAssessmentService _service;
    private readonly ILogger<CreateRiskAssessmentHandler> _logger;

    public CreateRiskAssessmentHandler(
        IRiskAssessmentService service,
        ILogger<CreateRiskAssessmentHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IResult> Handle(CreateRiskAssessmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RiskImpactDescription))
        {
            return Results.BadRequest(new { error = "RiskImpactDescription is required" });
        }

        _logger.LogInformation(
            "Creating RiskAssessment with description {RiskImpactDescription}",
            request.RiskImpactDescription
        );

        RiskAssessment riskAssessment = new RiskAssessment
        {
            RiskId = request.RiskId,
            RiskImpactDescription = request.RiskImpactDescription,
            ExistingControls = request.ExistingControls,
            Impact = request.Impact,
            Likelihood = request.Likelihood,
            RiskCategory = request.RiskCategory
        };

        riskAssessment = await _service.CreateAsync(riskAssessment);

        // var item = await _service.CreateAsync(
        //     request.RiskId,
        //     request.RiskImpactDescription,
        //     request.ExistingControls,
        //     request.Impact,
        //     request.Likelihood,
        //     request.RiskCategory
        // );

        return Results.Ok(riskAssessment);
    }
}