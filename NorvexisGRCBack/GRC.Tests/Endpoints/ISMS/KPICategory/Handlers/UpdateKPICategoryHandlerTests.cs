using FluentAssertions;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;
using GRC.Endpoints.ISMS;
using GRC.Endpoints.ISMS.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.ISMS.Handlers;

public class UpdateKPICategoryHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidRequest_ShouldReturnOk()
    {
        // Arrange
        var mock = new Mock<IKPICategoryService>();
        var logger = new Mock<ILogger<UpdateKPICategoryHandler>>();

        var handler = new UpdateKPICategoryHandler(mock.Object, logger.Object);

        var request = new UpdateKPICategoryRequest("Risk");

        var updated = new KPICategory
        {
            Id = "1",
            KPI_Category = "Risk"
        };

        mock.Setup(x => x.UpdateAsync(It.IsAny<KPICategory>()))
            .ReturnsAsync(updated);

        // Act
        var result = await handler.Handle("1", request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().Contain("Ok");

        mock.Verify(x => x.UpdateAsync(It.Is<KPICategory>(c =>
            c.Id == "1" &&
            c.KPI_Category == "Risk"
        )), Times.Once);
    }
}