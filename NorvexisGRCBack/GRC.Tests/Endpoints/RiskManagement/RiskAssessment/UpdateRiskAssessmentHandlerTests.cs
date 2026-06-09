using FluentAssertions;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Endpoints.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class UpdateRiskAssessmentHandlerTests
{
    [Fact]
    public async Task Handle_WhenRiskDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var riskServiceMock = new Mock<IRiskAssessmentService>();

        var handler = new UpdateRiskAssessmentHandler(riskServiceMock.Object);

        var request = new UpdateRiskAssessmentRequest(
            RiskId: "r-001", 
            RiskImpactDescription: "", 
            ExistingControls: "test",
            Impact: "objective",
            Likelihood: "test",
            RiskCategory: "test"
        );

        riskServiceMock
            .Setup(x => x.UpdateAsync(
                "missing-id",
                request.RiskId,
                request.RiskImpactDescription,
                request.ExistingControls,
                request.Impact,
                request.Likelihood,
                request.RiskCategory
                ))
            .ReturnsAsync((RiskAssessment?)null);

        // Act
        var result = await handler.Handle("missing-id",request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("NotFound");
    }

}