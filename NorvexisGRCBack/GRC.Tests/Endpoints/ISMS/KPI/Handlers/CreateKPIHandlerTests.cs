using FluentAssertions;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;
using GRC.Endpoints.ISMS;
using GRC.Endpoints.ISMS.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.ISMS.Handlers;

public class CreateKPIHandlerTests
{
    [Fact]
    public async Task Handle_WhenKpiNameIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var kpiServiceMock = new Mock<IKPIService>();
        var loggerMock = new Mock<ILogger<CreateKPIHandler>>();

        var handler = new CreateKPIHandler(kpiServiceMock.Object, loggerMock.Object);

        var request = new CreateKPIRequest(
           KPICategoryId: "cat-1",
           KpiName: "",
           KpiConfId: "tr4523-rqew434-dqaw341",
           KPIResponsibleId: "resp-1",
           Value: 10,
           TargetValue: 15,
           Comments: "comentario",
           PeriodDate: new DateTime(1, 1, 1)
       );

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IResult>();
        result.GetType().Name.Should().StartWith("BadRequest");

        kpiServiceMock.Verify(x => x.CreateAsync(It.IsAny<KPI>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCallServiceAndReturnOk()
    {
        //Arrange
        var kpiServiceMock = new Mock<IKPIService>();
        var loggerMock = new Mock<ILogger<CreateKPIHandler>>();

        var handler = new CreateKPIHandler(kpiServiceMock.Object, loggerMock.Object);

        var request = new CreateKPIRequest(
            KPICategoryId: "cat-1",
            KpiName: "KPI válido",
            KpiConfId: "tr4523-rqew434-dqaw341",
            KPIResponsibleId: "resp-1",
            Value: 10,
            TargetValue: 15,
            Comments: "comentario",
            PeriodDate: new DateTime(1, 1, 1)
        );

        var createdKpi = new KPI
        {
            Id = "kpi-1",
            KPI_Name = request.KpiName,
            KpiConfId = request.KpiConfId,
            KPICategoryId = request.KPICategoryId,
            KPIResponsibleId = request.KPIResponsibleId,
            Value = request.Value,
            TargetValue = request.TargetValue,
            Comments = request.Comments,
            PeriodDate = request.PeriodDate
        };

        kpiServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<KPI>()))
            .ReturnsAsync(createdKpi);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("Ok");

        kpiServiceMock.Verify(x => x.CreateAsync(It.Is<KPI>(k =>
            k.KPI_Name == request.KpiName &&
            k.KPICategoryId == request.KPICategoryId &&
            k.KPIResponsibleId == request.KPIResponsibleId &&
            k.Value == request.Value &&
            k.TargetValue == request.TargetValue &&
            k.Comments == request.Comments
        )), Times.Once);

    }
}