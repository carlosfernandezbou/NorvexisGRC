using FluentAssertions;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Endpoints.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class DeleteRiskTreatmentHandlerTests
{
    [Fact]
    public async Task Handle_WhenTittleIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var riskServiceMock = new Mock<IRiskTreatmentService>();
        riskServiceMock
                            .Setup(x => x.DeleteAsync("r-001"))
                            .ReturnsAsync((RiskTreatment?)null);

        var handler = new DeleteRiskTreatmentHandler(riskServiceMock.Object);

        // Act
        var result = await handler.Handle("r-001");

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("NotFound");

    }


}