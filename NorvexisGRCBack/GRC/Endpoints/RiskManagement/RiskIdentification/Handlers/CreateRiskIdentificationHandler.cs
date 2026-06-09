using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;

namespace GRC.Endpoints.RiskManagement.Handlers;

public class CreateRiskIdentificationHandler
{
    private readonly IRiskIdentificationService _service;
    private readonly ILogger<CreateRiskIdentificationHandler> _logger;

    public CreateRiskIdentificationHandler(
        IRiskIdentificationService service,
        ILogger<CreateRiskIdentificationHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IResult> Handle(CreateRiskIdentificationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AssetName))
        {
            return Results.BadRequest(new { error = "AssetName is required" });
        }

        _logger.LogInformation(
            "Creating RiskIdentification with asset {AssetName}",
            request.AssetName
        );

        RiskIdentification riskIdentification = new RiskIdentification
        {
            RiskId = request.RiskId,
            Category = request.Category,
            SubCategory = request.SubCategory,
            AssetType = request.AssetType,
            AssetName = request.AssetName,
            Vulnerability = request.Vulnerability,
            Threats = request.Threats,
            CoreValueImpacted = request.CoreValueImpacted
        };

        riskIdentification = await _service.CreateAsync(riskIdentification);

        // var item = await _service.CreateAsync(
        //     request.RiskId,
        //     request.Category,
        //     request.SubCategory,
        //     request.AssetType,
        //     request.AssetName,
        //     request.Vulnerability,
        //     request.Threats,
        //     request.CoreValueImpacted
        // );

        return Results.Ok(riskIdentification);
    }
}