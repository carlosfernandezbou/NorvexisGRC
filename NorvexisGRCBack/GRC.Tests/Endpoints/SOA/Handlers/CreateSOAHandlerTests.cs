using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using FluentAssertions;
using GRC.Core.Interfaces.SOA;
using SOAModel = GRC.Core.Models.SOA.SOA;
using GRC.Endpoints.SOA;
using GRC.Endpoints.SOA.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class CreateSOAHandlerTests
{
    [Fact]
    public async Task Handle_WhenTittleIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var soaServiceMock = new Mock<ISOAService>();
        var loggerMock = new Mock<ILogger<CreateSOAHandler>>();

        var handler = new CreateSOAHandler(soaServiceMock.Object, loggerMock.Object);

        var request = new CreateSOARequest(
            Section: "section", 
            Control: "control", 
            Tittle: "",
            Objective: "objective",
            Applicable: true,
            Implemented: false,
            Justification: "justification",
            Evidence: "evidence"
        );

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IResult>();
        result.GetType().Name.Should().StartWith("BadRequest");

        soaServiceMock.Verify(x => x.CreateAsync(It.IsAny<SOAModel>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCallServiceAndReturnOk()
    {
        //Arrange
        var soaServiceMock = new Mock<ISOAService>();
        var loggerMock = new Mock<ILogger<CreateSOAHandler>>();

        var handler = new CreateSOAHandler(soaServiceMock.Object, loggerMock.Object);

        var request = new CreateSOARequest(
            Section: "section", 
            Control: "control", 
            Tittle: "title",
            Objective: "objective",
            Applicable: true,
            Implemented: false,
            Justification: "justification",
            Evidence: "evidence"
        );

        var createdSoa = new SOAModel
        {
            Id = "1",
            Section = request.Section, 
            Control = request.Control, 
            Tittle = request.Tittle,
            Objective = request.Objective,
            Applicable = true,
            Implemented = false,
            Justification = request.Justification,
            Evidence = request.Evidence
        };

        soaServiceMock.Setup(x => x.CreateAsync(It.Is<SOAModel>(s =>
                s.Section == request.Section &&
                s.Control == request.Control &&
                s.Tittle == request.Tittle &&
                s.Objective == request.Objective &&
                s.Applicable == request.Applicable &&
                s.Implemented == request.Implemented &&
                s.Justification == request.Justification &&
                s.Evidence == request.Evidence)))
            .ReturnsAsync(createdSoa);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("Ok");

        soaServiceMock.Verify(x => x.CreateAsync(It.Is<SOAModel>(s =>
            s.Section == request.Section &&
            s.Control == request.Control &&
            s.Tittle == request.Tittle &&
            s.Objective == request.Objective &&
            s.Applicable == request.Applicable &&
            s.Implemented == request.Implemented &&
            s.Justification == request.Justification &&
            s.Evidence == request.Evidence)), Times.Once);
    }
}