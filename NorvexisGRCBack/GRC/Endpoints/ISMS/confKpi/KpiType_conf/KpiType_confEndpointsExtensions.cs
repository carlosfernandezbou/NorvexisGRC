using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Endpoints.ISMS.confKpi.Handlers;

namespace GRC.Endpoints.ISMS.confKpi;

public static class KpiType_confEndpointsExtensions
{
    public static IEndpointRouteBuilder MapKpiType_confEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/kpitype_conf");

        group.MapGet("/", async (IKpiType_conf kpiService) =>
        {
           var kpis = await kpiService.GetAllAsync();
           return Results.Ok(kpis); 
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapGet("/{id}", async (string id, IKpiType_conf kpiService) =>
        {
            var kpi = await kpiService.GetOneAsync(id);

            if (kpi == null)
                return Results.NotFound(new { error = "KPI not found" });

            return Results.Ok(kpi);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/", async (CreateKpiType_confRequest request, CreateKpiType_confHandler handler) =>
        {
            return await handler.Handle(request);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/{id}", async (string id, DeleteKpiType_confHandler handler) =>
        {
            return await handler.Handle(id);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPut("/{id}", async (string id, UpdateKpiType_confRequest request, UpdateKpiType_confHandler handler) =>
        {
            return await handler.Handle(id, request);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        return app;
    }
}