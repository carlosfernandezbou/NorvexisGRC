using FluentAssertions;
using GRC.Core.Interfaces.SOA;
using SOAModel = GRC.Core.Models.SOA.SOA;
using GRC.Endpoints.SOA;
using GRC.Endpoints.SOA.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GRC.Tests.Endpoints.SOA.Handlers;

public class UpdateSOAHandlerTests
{
    [Fact]
    public async Task Handle_WhenSOADoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var kpiServiceMock = new Mock<ISOAService>();

        var handler = new UpdateSOAHandler(kpiServiceMock.Object);

        var request = new UpdateSOARequest(
            Section: "cat-1",
            Control: "Nuevo nombre",
            Tittle: "resp-1",
            Objective: "10",
            Applicable: true,
            Implemented: false,
            Justification: "justification",
            Evidence: "evidence"
        );

        kpiServiceMock
            .Setup(x => x.UpdateAsync(
                "missing-id",
                request.Section,
                request.Control,
                request.Tittle,
                request.Objective,
                request.Applicable,
                request.Implemented,
                request.Justification,
                request.Evidence
                ))
            .ReturnsAsync((SOAModel?)null);

        // Act
        var result = await handler.Handle("missing-id", request);

        // Assert
        result.Should().NotBeNull();
        result.GetType().Name.Should().StartWith("NotFound");
    }

}