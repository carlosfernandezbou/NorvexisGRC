using FluentAssertions;
using GRC.Core.Interfaces.SOA;
using SOAModel = GRC.Core.Models.SOA.SOA;
using GRC.Endpoints.SOA;
using GRC.Endpoints.SOA.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class DeleteSOAHandlerTests
{
    [Fact]
    public async Task Handle_WhenSOADoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var soaServiceMock = new Mock<ISOAService>();
        soaServiceMock
                    .Setup(x => x.DeleteAsync("1"))
                    .ReturnsAsync((SOAModel?)null);

        var handler = new DeleteSOAHandler(soaServiceMock.Object);

        // Act
        var result = await handler.Handle("1");

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("NotFound");

        soaServiceMock.Verify(x => x.DeleteAsync("1"), Times.Once);
    }

}