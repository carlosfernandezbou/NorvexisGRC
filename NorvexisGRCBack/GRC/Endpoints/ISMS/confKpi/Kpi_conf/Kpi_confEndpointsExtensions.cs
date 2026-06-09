using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Endpoints.ISMS.confKpi.Handlers;

namespace GRC.Endpoints.ISMS.confKpi;

public static class Kpi_confEndpointsExtensions
{
    public static IEndpointRouteBuilder MapKpi_confEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/kpi_conf");

        group.MapGet("/", async (IKpi_conf kpiService) =>
        {
           var kpis = await kpiService.GetAllAsync();
           return Results.Ok(kpis); 
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapGet("/{id}", async (string id, IKpi_conf kpiService) =>
        {
            var kpi = await kpiService.GetOneAsync(id);

            if (kpi == null)
                return Results.NotFound(new { error = "KPI not found" });

            return Results.Ok(kpi);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/", async (CreateKpi_confRequest request, CreateKpi_confHandler handler) =>
        {
            return await handler.Handle(request);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/{id}", async (string id, DeleteKpi_confHandler handler) =>
        {
            return await handler.Handle(id);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPut("/{id}", async (string id, UpdateKpi_confRequest request, UpdateKpi_confHandler handler) =>
        {
            return await handler.Handle(id, request);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        return app;
    }
}