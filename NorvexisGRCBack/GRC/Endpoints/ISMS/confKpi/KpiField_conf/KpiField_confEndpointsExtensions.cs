using GRC.Core.Interfaces.ISMS.confKpi;
using GRC.Endpoints.ISMS.confKpi.Handlers;

namespace GRC.Endpoints.ISMS.confKpi;

public static class KpiField_confEndpointsExtensions
{
    public static IEndpointRouteBuilder MapKpiField_confEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/kpifield_conf");

        group.MapGet("/", async (IKpiField_conf kpiService) =>
        {
           var kpis = await kpiService.GetAllAsync();
           return Results.Ok(kpis); 
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapGet("/{id}", async (string id, IKpiField_conf kpiService) =>
        {
            var kpi = await kpiService.GetOneAsync(id);

            if (kpi == null)
                return Results.NotFound(new { error = "KPI not found" });

            return Results.Ok(kpi);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/", async (CreateKpiField_confRequest request, CreateKpiField_confHandler handler) =>
        {
            return await handler.Handle(request);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/{id}", async (string id, DeleteKpiField_confHandler handler) =>
        {
            return await handler.Handle(id);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPut("/{id}", async (string id, UpdateKpiField_confRequest request, UpdateKpiField_confHandler handler) =>
        {
            return await handler.Handle(id, request);
        }).WithTags("KpiConf").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        return app;
    }
}