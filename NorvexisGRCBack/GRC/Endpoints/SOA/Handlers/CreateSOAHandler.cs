using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.SOA;
using SOAModel = GRC.Core.Models.SOA.SOA;

namespace GRC.Endpoints.SOA.Handlers;

public class CreateSOAHandler
{
    private readonly ISOAService _soaService;
    private readonly ILogger<CreateSOAHandler> _logger;
    public CreateSOAHandler(ISOAService soaService, ILogger<CreateSOAHandler> logger)
    {
        _soaService = soaService;
        _logger = logger;
    }
    
    public async Task<IResult> Handle(CreateSOARequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Tittle))
        {
            return Results.BadRequest(new { error = "Tittle is required" });
        }

        _logger.LogInformation("Creating SOA with title {SOAName}", request.Tittle);
        
        SOAModel soa = new SOAModel
        {
            Section = request.Section,
            Control = request.Control,
            Tittle = request.Tittle,
            Objective = request.Objective,
            Applicable = request.Applicable,
            Implemented = request.Implemented,
            Justification = request.Justification,
            Evidence = request.Evidence
        };

        soa = await _soaService.CreateAsync(soa);

        // var soa = await _soaService.CreateAsync(
        //     request.Section,
        //     request.Control,
        //     request.Tittle,
        //     request.Objective,
        //     request.Applicable,
        //     request.Implemented,
        //     request.Justification,
        //     request.Evidence
        // );

        return Results.Ok(soa);
    }
}