using FluentAssertions;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;
using GRC.Endpoints.ISMS;
using GRC.Endpoints.ISMS.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.ISMS.Handlers;

public class UpdateKPIResponsibleHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidRequest_ShouldReturnOk()
    {
        //Arrange
        var mock = new Mock<IKPIResponsibleService>();
        var logger = new Mock<ILogger<UpdateKPIResponsibleHandler>>();

        var handler = new UpdateKPIResponsibleHandler(mock.Object, logger.Object);

        var request = new UpdateKPIResponsibleRequest("Risk");

        var updated = new KPIResponsible
        {
            Id = "1",
            Responsible = "Risk"
        };

        mock.Setup(x => x.UpdateAsync(It.IsAny<KPIResponsible>()))
            .ReturnsAsync(updated);
        // Act
        var result = await handler.Handle("1", request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().Contain("Ok");

        mock.Verify(x => x.UpdateAsync(It.Is<KPIResponsible>(c =>
            c.Id == "1" &&
            c.Responsible == "Risk"
        )), Times.Once);
    }

}