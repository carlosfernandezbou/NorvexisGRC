using FluentAssertions;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Endpoints.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class UpdateRiskIdentificationHandlerTests
{
    [Fact]
    public async Task Handle_WhenRiskDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var riskServiceMock = new Mock<IRiskIdentificationService>();

        var handler = new UpdateRiskIdentificationHandler(riskServiceMock.Object);

        var request = new UpdateRiskIdentificationRequest(
            RiskId: "r-001",
            Category: "test",
            SubCategory: "test",
            AssetType: "objective",
            AssetName: "",
            Vulnerability: "test",
            Threats: "test",
            CoreValueImpacted: "test"
        );

        riskServiceMock
            .Setup(x => x.UpdateAsync(
                "missing-id",
                request.RiskId,
                request.Category,
                request.SubCategory,
                request.AssetType,
                request.AssetName,
                request.Vulnerability,
                request.Threats,
                request.CoreValueImpacted
                ))
            .ReturnsAsync((RiskIdentification?)null);

        // Act
        var result = await handler.Handle("missing-id",request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("NotFound");
    }

}