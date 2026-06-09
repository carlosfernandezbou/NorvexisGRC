using FluentAssertions;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Endpoints.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class CreateRiskAssessmentHandlerTests
{
    [Fact]
    public async Task Handle_WhenTittleIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var riskServiceMock = new Mock<IRiskAssessmentService>();
        var loggerMock = new Mock<ILogger<CreateRiskAssessmentHandler>>();

        var handler = new CreateRiskAssessmentHandler(riskServiceMock.Object, loggerMock.Object);

        var request = new CreateRiskAssessmentRequest(
            RiskId: "r-001",
            RiskImpactDescription: "",
            ExistingControls: "test",
            Impact: "objective",
            Likelihood: "test",
            RiskCategory: "test"
        );

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IResult>();
        result.GetType().Name.Should().StartWith("BadRequest");

        riskServiceMock.Verify(x => x.CreateAsync(It.IsAny<RiskAssessment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCallServiceAndReturnOk()
    {
        //Arrange
        var riskServiceMock = new Mock<IRiskAssessmentService>();
        var loggerMock = new Mock<ILogger<CreateRiskAssessmentHandler>>();

        var handler = new CreateRiskAssessmentHandler(riskServiceMock.Object, loggerMock.Object);

        var request = new CreateRiskAssessmentRequest(
            RiskId: "r-001",
            RiskImpactDescription: "test",
            ExistingControls: "test",
            Impact: "objective",
            Likelihood: "test",
            RiskCategory: "test"
        );

        var createdSoa = new RiskAssessment
        {
            Id = "1",
            RiskId = "r-001",
            RiskImpactDescription = "test",
            ExistingControls = "test",
            Impact = "objective",
            Likelihood = "test",
            RiskCategory = "test"
        };

        riskServiceMock
            .Setup(x => x.CreateAsync(It.Is<RiskAssessment>(r =>
                r.RiskId == request.RiskId &&
                r.RiskImpactDescription == request.RiskImpactDescription &&
                r.ExistingControls == request.ExistingControls &&
                r.Impact == request.Impact &&
                r.Likelihood == request.Likelihood &&
                r.RiskCategory == request.RiskCategory)))
            .ReturnsAsync(createdSoa);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("Ok");

        riskServiceMock.Verify(x => x.CreateAsync(It.Is<RiskAssessment>(r =>
            r.RiskId == request.RiskId &&
            r.RiskImpactDescription == request.RiskImpactDescription &&
            r.ExistingControls == request.ExistingControls &&
            r.Impact == request.Impact &&
            r.Likelihood == request.Likelihood &&
            r.RiskCategory == request.RiskCategory)), Times.Once);
    }
}