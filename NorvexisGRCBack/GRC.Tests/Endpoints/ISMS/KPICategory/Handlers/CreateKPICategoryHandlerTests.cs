using FluentAssertions;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;
using GRC.Endpoints.ISMS;
using GRC.Endpoints.ISMS.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.ISMS.Handlers;

public class CreateKPICategoryHandlerTests
{
    [Fact]
    public async Task Handle_WhenKpiNameIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var kpiServiceMock = new Mock<IKPICategoryService>();
        var loggerMock = new Mock<ILogger<CreateKPICategoryHandler>>();

        var handler = new CreateKPICategoryHandler(kpiServiceMock.Object, loggerMock.Object);

        var request = new CreateKPICategoryRequest(KPICategory: "");

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IResult>();
        result.GetType().Name.Should().StartWith("BadRequest");

        kpiServiceMock.Verify(x => x.CreateAsync(It.IsAny<KPICategory>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCallServiceAndReturnOk()
    {
        // Arrange
        var kpiServiceMock = new Mock<IKPICategoryService>();
        var loggerMock = new Mock<ILogger<CreateKPICategoryHandler>>();

        var handler = new CreateKPICategoryHandler(kpiServiceMock.Object, loggerMock.Object);

        var request = new CreateKPICategoryRequest(
            KPICategory: "cat-1"
        );

        var createdCategory = new KPICategory
        {
            Id = "kpi-1",
            KPI_Category = "cat-1"
        };

        kpiServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<KPICategory>()))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("Ok");

        kpiServiceMock.Verify(x => x.CreateAsync(It.Is<KPICategory>(c =>
            c.KPI_Category == request.KPICategory
        )), Times.Once);
    }
}