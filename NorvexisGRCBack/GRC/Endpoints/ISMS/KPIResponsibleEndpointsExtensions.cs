using GRC.Core.Interfaces.ISMS;
using GRC.Endpoints.ISMS.Handlers;

namespace GRC.Endpoints.ISMS;

public static class KPIResponsibleEndpointsExtensions
{
    public static IEndpointRouteBuilder MapKPIResponsibleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/kpi-responsibles");

        group.MapGet("/", async (IKPIResponsibleService service) =>
        {
            var responsibles = await service.GetAllAsync();
            return Results.Ok(responsibles);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapGet("/{id}", async (string id, IKPIResponsibleService service) =>
        {
            var responsible = await service.GetOneAsync(id);

            if (responsible is null)
                return Results.NotFound(new { error = "KPI Responsible not found" });

            return Results.Ok(responsible);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/", async (CreateKPIResponsibleRequest request, CreateKPIResponsibleHandler handler) =>
        {
            return await handler.Handle(request);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPut("/{id}", async (string id, UpdateKPIResponsibleRequest request, UpdateKPIResponsibleHandler handler) =>
        {
            return await handler.Handle(id, request);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/{id}", async (string id, DeleteKPIResponsibleHandler handler) =>
        {
            return await handler.Handle(id);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        return app;
    }
}