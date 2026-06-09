using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.SOA;
using SOAModel = GRC.Core.Models.SOA.SOA;

namespace GRC.Endpoints.SOA.Handlers;

public class UpdateSOAHandler
{
    private readonly ISOAService _soaService;
    public UpdateSOAHandler(ISOAService soaService)
    {
        _soaService = soaService;
    }
    
    public async Task<IResult> Handle(string id, UpdateSOARequest request)
    {
        // var soa = await _soaService.UpdateAsync(
        //     id,
        //     request.Section,
        //     request.Control,
        //     request.Tittle,
        //     request.Objective,
        //     request.Applicable,
        //     request.Implemented,
        //     request.Justification,
        //     request.Evidence
        // );

        SOAModel soa = new SOAModel
        {
            Id = id,
            Section = request.Section,
            Control = request.Control,
            Tittle = request.Tittle,
            Objective = request.Objective,
            Applicable = request.Applicable,
            Implemented = request.Implemented,
            Justification = request.Justification,
            Evidence = request.Evidence
        };

        soa = await _soaService.UpdateAsync(soa);

        if (soa == null)
        {
            return Results.NotFound(new { error = "SOA not found" });
        }

        return Results.Ok(soa);
    }
}