using GRC.Core.Interfaces.SOA;
using GRC.Endpoints.SOA.Handlers;

namespace GRC.Endpoints.SOA;

public static class SOAEndpointsExtensions
{
    public static IEndpointRouteBuilder MapSOAEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/soas");

        group.MapGet("/", async (ISOAService soaService) =>
        {
            var soas = await soaService.GetAllAsync();
            return Results.Ok(soas);
        }).WithTags("SOA").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapGet("/{id}", async (string id, ISOAService soaService) =>
        {
            var soa = await soaService.GetOneAsync(id);

            if (soa == null)
                return Results.NotFound(new { error = "SOA not found" });

            return Results.Ok(soa);
        }).WithTags("SOA").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/", async (CreateSOARequest request, CreateSOAHandler handler) =>
        {
            return await handler.Handle(request);
        }).WithTags("SOA").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/{id}", async (string id, DeleteSOAHandler handler) =>
        {
            return await handler.Handle(id);
        }).WithTags("SOA").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPut("/{id}", async (string id, UpdateSOARequest request, UpdateSOAHandler handler) =>
        {
            return await handler.Handle(id, request);
        }).WithTags("SOA").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));


        group.MapPost("/import-json", async (IFormFile file, ISOAService soaService) =>
{
            if (file == null || file.Length == 0)
                return Results.BadRequest(new { error = "No se ha enviado ningún archivo." });
        
            if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                return Results.BadRequest(new { error = "El archivo debe ser un .json" });
        
            using var stream = file.OpenReadStream();
            var imported = await soaService.ImportFromJsonAsync(stream);
        
            return Results.Ok(new
            {
                message = "Importación completada",
                importedRows = imported
            });
        })
        .WithTags("SOA")
        .DisableAntiforgery()
        .RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));
        

        return app;
    }
}