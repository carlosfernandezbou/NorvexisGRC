using GRC.Core.Interfaces.ISMS;
using GRC.Endpoints.ISMS.Handlers;

namespace GRC.Endpoints.ISMS;

public static class KPICategoryEndpointsExtensions
{
    public static IEndpointRouteBuilder MapKPICategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/kpi-categories");

        group.MapGet("/", async (IKPICategoryService service) =>
{
    var items = await service.GetAllAsync();
    return Results.Ok(items);
}).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapGet("/{id}", async (string id, IKPICategoryService service) =>
        {
            var item = await service.GetOneAsync(id);

            if (item is null)
                return Results.NotFound(new { error = "KPI Category not found" });

            return Results.Ok(item);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/", async (CreateKPICategoryRequest request, CreateKPICategoryHandler handler) =>
        {
            return await handler.Handle(request);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/{id}", async (string id, DeleteKPICategoryHandler handler) =>
        {
            return await handler.Handle(id);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPut("/{id}", async (string id, UpdateKPICategoryRequest request, UpdateKPICategoryHandler handler) =>
        {
            return await handler.Handle(id, request);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        return app;
    }
}