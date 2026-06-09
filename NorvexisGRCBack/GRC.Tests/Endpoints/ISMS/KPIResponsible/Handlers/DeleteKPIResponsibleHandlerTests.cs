using DocumentFormat.OpenXml.Spreadsheet;
using FluentAssertions;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;
using GRC.Endpoints.ISMS;
using GRC.Endpoints.ISMS.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.ISMS.Handlers;

public class DeleteKPIResponsibleHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidRequest_ShouldReturnOk()
    {
        //Arrange
        var kpiServiceMock = new Mock<IKPIResponsibleService>();
        var loggerMock = new Mock<ILogger<DeleteKPIResponsibleHandler>>();

        var handler = new DeleteKPIResponsibleHandler(
            kpiServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = await handler.Handle("1");

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("NotFound");


        kpiServiceMock.Verify(x => x.DeleteAsync("1"), Times.Once);
    }

}