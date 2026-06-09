using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.SOA;
using GRC.Core.Models.SOA;

namespace GRC.Endpoints.SOA.Handlers;

public class DeleteSOAHandler
{
    private readonly ISOAService _soaService;
    public DeleteSOAHandler(ISOAService soaService)
    {
        _soaService = soaService;
    }
    
    public async Task<IResult> Handle(string id)
    {
        var soa = await _soaService.DeleteAsync(id);

        if (soa == null)
        {
            return Results.NotFound(new { error = "soa not found" });
        }
        
        return Results.Ok(soa);
    }
}