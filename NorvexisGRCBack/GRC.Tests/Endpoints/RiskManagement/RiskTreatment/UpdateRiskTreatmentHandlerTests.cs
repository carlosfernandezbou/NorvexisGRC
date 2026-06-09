using FluentAssertions;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Endpoints.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class UpdateRiskTreatmentHandlerTests
{
    [Fact]
    public async Task Handle_WhenTittleIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var riskServiceMock = new Mock<IRiskTreatmentService>();

        var handler = new UpdateRiskTreatmentHandler(riskServiceMock.Object);

        var request = new UpdateRiskTreatmentRequest(
            RiskId: "r-001",
            RiskTreatmentOption: "test",
            TreatmentPlan: "test",
            Owner: "objective",
            DueDate: "test",
            Status: "test",
            ResidualRisk: "test",
            Comments: "test"
        );

        riskServiceMock
            .Setup(x => x.UpdateAsync(
                "missing-id",
                request.RiskId,
                request.RiskTreatmentOption,
                request.TreatmentPlan,
                request.Owner,
                request.DueDate,
                request.Status,
                request.ResidualRisk,
                request.Comments
            )).ReturnsAsync((RiskTreatment?)null);

        // Act
        var result = await handler.Handle("missing-id", request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("NotFound");

    }


}