using FluentAssertions;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;
using GRC.Endpoints.ISMS;
using GRC.Endpoints.ISMS.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.ISMS.Handlers;

public class CreateKPIResponsibleHandlerTests
{
    [Fact]
    public async Task Handle_WhenKpiNameIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var kpiServiceMock = new Mock<IKPIResponsibleService>();
        var loggerMock = new Mock<ILogger<CreateKPIResponsibleHandler>>();

        var handler = new CreateKPIResponsibleHandler(kpiServiceMock.Object, loggerMock.Object);

        var request = new CreateKPIResponsibleRequest(Responsible: "");

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IResult>();
        result.GetType().Name.Should().StartWith("BadRequest");

        kpiServiceMock.Verify(x => x.CreateAsync(It.IsAny<KPIResponsible>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCallServiceAndReturnOk()
    {
        // Arrange
        var kpiServiceMock = new Mock<IKPIResponsibleService>();
        var loggerMock = new Mock<ILogger<CreateKPIResponsibleHandler>>();

        var handler = new CreateKPIResponsibleHandler(kpiServiceMock.Object, loggerMock.Object);

        var request = new CreateKPIResponsibleRequest(
            Responsible: "cat-1"
        );

        var createdResponsible = new KPIResponsible
        {
            Id = "kpi-1",
            Responsible = "cat-1"
        };

        kpiServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<KPIResponsible>()))
            .ReturnsAsync(createdResponsible);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("Ok");

        kpiServiceMock.Verify(x => x.CreateAsync(It.Is<KPIResponsible>(r =>
            r.Responsible == request.Responsible
        )), Times.Once);
    }
}