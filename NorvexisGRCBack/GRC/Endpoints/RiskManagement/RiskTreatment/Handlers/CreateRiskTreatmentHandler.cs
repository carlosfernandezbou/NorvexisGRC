using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;

namespace GRC.Endpoints.RiskManagement.Handlers;

public class CreateRiskTreatmentHandler
{
    private readonly IRiskTreatmentService _service;
    private readonly ILogger<CreateRiskTreatmentHandler> _logger;

    public CreateRiskTreatmentHandler(
        IRiskTreatmentService service,
        ILogger<CreateRiskTreatmentHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IResult> Handle(CreateRiskTreatmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RiskTreatmentOption))
        {
            return Results.BadRequest(new { error = "RiskTreatmentOption is required" });
        }

        _logger.LogInformation(
            "Creating RiskTreatment with option {RiskTreatmentOption}",
            request.RiskTreatmentOption
        );
        RiskTreatment riskTreatment = new RiskTreatment
        {
            RiskId = request.RiskId,
            RiskTreatmentOption = request.RiskTreatmentOption,
            TreatmentPlan = request.TreatmentPlan,
            Owner = request.Owner,
            DueDate = request.DueDate,
            Status = request.Status,
            ResidualRisk = request.ResidualRisk,
            Comments = request.Comments
        };

        riskTreatment = await _service.CreateAsync(riskTreatment);

        // var item = await _service.CreateAsync(
        //     request.RiskId,
        //     request.RiskTreatmentOption,
        //     request.TreatmentPlan,
        //     request.Owner,
        //     request.DueDate,
        //     request.Status,
        //     request.ResidualRisk,
        //     request.Comments
        // );

        return Results.Ok(riskTreatment);
    }
}