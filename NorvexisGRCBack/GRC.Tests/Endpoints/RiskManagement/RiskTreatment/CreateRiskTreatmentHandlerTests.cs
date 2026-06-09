using FluentAssertions;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Endpoints.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class CreateRiskTreatmentHandlerTests
{
    [Fact]
    public async Task Handle_WhenTittleIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var riskServiceMock = new Mock<IRiskTreatmentService>();
        var loggerMock = new Mock<ILogger<CreateRiskTreatmentHandler>>();

        var handler = new CreateRiskTreatmentHandler(riskServiceMock.Object, loggerMock.Object);

        var request = new CreateRiskTreatmentRequest(
            RiskId: "r-001",
            RiskTreatmentOption: "",
            TreatmentPlan: "test",
            Owner: "objective",
            DueDate: "",
            Status: "test",
            ResidualRisk: "test",
            Comments: "test"
        );

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IResult>();
        result.GetType().Name.Should().StartWith("BadRequest");

        riskServiceMock.Verify(x => x.CreateAsync(It.IsAny<RiskTreatment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCallServiceAndReturnOk()
    {
        //Arrange
        var riskServiceMock = new Mock<IRiskTreatmentService>();
        var loggerMock = new Mock<ILogger<CreateRiskTreatmentHandler>>();

        var handler = new CreateRiskTreatmentHandler(riskServiceMock.Object, loggerMock.Object);

        var request = new CreateRiskTreatmentRequest(
            RiskId: "r-001",
            RiskTreatmentOption: "test",
            TreatmentPlan: "test",
            Owner: "objective",
            DueDate: "",
            Status: "test",
            ResidualRisk: "test",
            Comments: "test"
        );

        var createdSoa = new RiskTreatment
        {
            Id = "1",
            RiskId = "r-001",
            RiskTreatmentOption = "test",
            TreatmentPlan = "test",
            Owner = "objective",
            DueDate = "",
            Status = "test",
            ResidualRisk = "test",
            Comments = "test"
        };

        riskServiceMock
            .Setup(x => x.CreateAsync(It.Is<RiskTreatment>(r =>
                r.RiskId == request.RiskId &&
                r.RiskTreatmentOption == request.RiskTreatmentOption &&
                r.TreatmentPlan == request.TreatmentPlan &&
                r.Owner == request.Owner &&
                r.DueDate == request.DueDate &&
                r.Status == request.Status &&
                r.ResidualRisk == request.ResidualRisk &&
                r.Comments == request.Comments)))
            .ReturnsAsync(createdSoa);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("Ok");

        riskServiceMock.Verify(x => x.CreateAsync(It.Is<RiskTreatment>(r =>
            r.RiskId == request.RiskId &&
            r.RiskTreatmentOption == request.RiskTreatmentOption &&
            r.TreatmentPlan == request.TreatmentPlan &&
            r.Owner == request.Owner &&
            r.DueDate == request.DueDate &&
            r.Status == request.Status &&
            r.ResidualRisk == request.ResidualRisk &&
            r.Comments == request.Comments)), Times.Once);
    }
}