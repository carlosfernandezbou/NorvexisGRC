using FluentAssertions;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;
using GRC.Endpoints.ISMS;
using GRC.Endpoints.ISMS.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.ISMS.Handlers;

public class UpdateKPIHandlerTests
{
    [Fact]
    public async Task Handle_WhenKpiDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var kpiServiceMock = new Mock<IKPIService>();

        var handler = new UpdateKPIHandler(kpiServiceMock.Object);

        var updatedKpi = new UpdateKPIRequest(
            KPICategoryId: "cat-1",
            KpiName: "Nuevo nombre",
            KPIResponsibleId: "resp-1",
            Value: 10,
            TargetValue: 15,
            Comments: "comentario"
        );

        kpiServiceMock.Setup(x => x.UpdateAsync(It.IsAny<KPI>()))
        .ReturnsAsync((KPI)null!);

        // Act
        var result = await handler.Handle("missing-id", updatedKpi);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("NotFound");
    }

}