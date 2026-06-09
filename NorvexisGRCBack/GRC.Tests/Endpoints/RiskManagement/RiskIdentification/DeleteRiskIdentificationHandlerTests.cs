using FluentAssertions;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Endpoints.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class DeleteRiskIdentificationHandlerTests
{
    [Fact]
    public async Task Handle_WhenRiskDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var riskServiceMock = new Mock<IRiskIdentificationService>();
        riskServiceMock
                    .Setup(x => x.DeleteAsync("1"))
                    .ReturnsAsync((RiskIdentification?)null);

        var handler = new DeleteRiskIdentificationHandler(riskServiceMock.Object);

        // Act
        var result = await handler.Handle("1");

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("NotFound");

        riskServiceMock.Verify(x => x.DeleteAsync("1"), Times.Once);

    }
}