using GRC.Core.Interfaces.ISMS;
using GRC.Endpoints.ISMS.Handlers;

namespace GRC.Endpoints.ISMS;

public static class KPIEndpointsExtensions
{
    public static IEndpointRouteBuilder MapKPIEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/kpis");

        group.MapGet("/", async (IKPIService kpiService) =>
        {
            var kpis = await kpiService.GetAllAsync();
            return Results.Ok(kpis);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapGet("/{id}", async (string id, IKPIService kpiService) =>
        {
            var kpi = await kpiService.GetOneAsync(id);

            if (kpi == null)
                return Results.NotFound(new { error = "KPI not found" });

            return Results.Ok(kpi);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/", async (CreateKPIRequest request, CreateKPIHandler handler) =>
        {
            return await handler.Handle(request);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPut("/{id}", async (string id, UpdateKPIRequest request, UpdateKPIHandler handler) =>
        {
            return await handler.Handle(id, request);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/{id}", async (string id, DeleteKPIHandler handler) =>
        {
            return await handler.Handle(id);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/month/{year:int}/{month:int}", async (int year, int month, DeleteOneMonthKPIHandler handler) =>
        {
            var request = new DeleteOneMonthKPIRequest(year, month);
            return await handler.Handle(request);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/generate/{year:int}/{month:int}", async (int year, int month, GenerateKpisHandler handler) =>
        {
            var request = new GenerateKpisRequest(year, month);
            return await handler.Handle(request);
        }).WithTags("ISMS").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        return app;
    }
}