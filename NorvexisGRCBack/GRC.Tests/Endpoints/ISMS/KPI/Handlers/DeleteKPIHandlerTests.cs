using FluentAssertions;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;
using GRC.Endpoints.ISMS;
using GRC.Endpoints.ISMS.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.ISMS.Handlers;

public class DeleteKPIHandlerTests
{
    [Fact]
    public async Task Handle_WhenKpiDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var kpiServiceMock = new Mock<IKPIService>();
        kpiServiceMock
                   .Setup(x => x.DeleteAsync("1"))
                   .ReturnsAsync((KPI?)null);

        var handler = new DeleteKPIHandler(kpiServiceMock.Object);

        //Act
        var result = await handler.Handle("1");

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("NotFound");

        kpiServiceMock.Verify(x => x.DeleteAsync("1"), Times.Once);
    }
}
