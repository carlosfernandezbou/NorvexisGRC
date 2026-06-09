using FluentAssertions;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Endpoints.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class CreateRiskIdentificationHandlerTests
{
    [Fact]
    public async Task Handle_WhenTittleIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var riskServiceMock = new Mock<IRiskIdentificationService>();
        var loggerMock = new Mock<ILogger<CreateRiskIdentificationHandler>>();

        var handler = new CreateRiskIdentificationHandler(riskServiceMock.Object, loggerMock.Object);

        var request = new CreateRiskIdentificationRequest(
            RiskId: "r-001",
            Category: "test",
            SubCategory: "test",
            AssetType: "objective",
            AssetName: "",
            Vulnerability: "test",
            Threats: "test",
            CoreValueImpacted: "test"
        );

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IResult>();
        result.GetType().Name.Should().StartWith("BadRequest");

        riskServiceMock.Verify(x => x.CreateAsync(It.IsAny<RiskIdentification>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCallServiceAndReturnOk()
    {
        //Arrange
        var riskServiceMock = new Mock<IRiskIdentificationService>();
        var loggerMock = new Mock<ILogger<CreateRiskIdentificationHandler>>();

        var handler = new CreateRiskIdentificationHandler(riskServiceMock.Object, loggerMock.Object);

        var request = new CreateRiskIdentificationRequest(
            RiskId: "r-001",
            Category: "test",
            SubCategory: "test",
            AssetType: "objective",
            AssetName: "test",
            Vulnerability: "test",
            Threats: "test",
            CoreValueImpacted: "test"
        );

        var createdSoa = new RiskIdentification
        {
            Id = "1",
            RiskId = "r-001",
            Category = "test",
            SubCategory = "test",
            AssetType = "objective",
            AssetName = "test",
            Vulnerability = "test",
            Threats = "test",
            CoreValueImpacted = "test"
        };

        riskServiceMock
            .Setup(x => x.CreateAsync(It.Is<RiskIdentification>(r =>
                r.RiskId == request.RiskId &&
                r.Category == request.Category &&
                r.SubCategory == request.SubCategory &&
                r.AssetType == request.AssetType &&
                r.AssetName == request.AssetName &&
                r.Vulnerability == request.Vulnerability &&
                r.Threats == request.Threats &&
                r.CoreValueImpacted == request.CoreValueImpacted)))
            .ReturnsAsync(createdSoa);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("Ok");

        riskServiceMock.Verify(x => x.CreateAsync(It.Is<RiskIdentification>(r =>
            r.RiskId == request.RiskId &&
            r.Category == request.Category &&
            r.SubCategory == request.SubCategory &&
            r.AssetType == request.AssetType &&
            r.AssetName == request.AssetName &&
            r.Vulnerability == request.Vulnerability &&
            r.Threats == request.Threats &&
            r.CoreValueImpacted == request.CoreValueImpacted)), Times.Once);
    }
}