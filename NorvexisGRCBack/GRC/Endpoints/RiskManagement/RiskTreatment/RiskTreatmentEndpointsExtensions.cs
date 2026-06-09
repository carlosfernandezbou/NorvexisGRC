using GRC.Core.Interfaces.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;

namespace GRC.Endpoints.RiskManagement;

public static class RiskTreatmentEndpointsExtensions
{
    public static IEndpointRouteBuilder MapRiskTreatmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/risk-treatments");

        group.MapGet("/", async (IRiskTreatmentService riskTreatmentService) =>
        {
            var riskTreatments = await riskTreatmentService.GetAllAsync();
            return Results.Ok(riskTreatments);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapGet("/{id}", async (string id, IRiskTreatmentService riskTreatmentService) =>
        {
            var riskTreatment = await riskTreatmentService.GetOneAsync(id);

            if (riskTreatment == null)
                return Results.NotFound(new { error = "RiskTreatment not found" });

            return Results.Ok(riskTreatment);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/", async (CreateRiskTreatmentRequest request, CreateRiskTreatmentHandler handler) =>
        {
            return await handler.Handle(request);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/{id}", async (string id, DeleteRiskTreatmentHandler handler) =>
        {
            return await handler.Handle(id);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPut("/{id}", async (string id, UpdateRiskTreatmentRequest request, UpdateRiskTreatmentHandler handler) =>
        {
            return await handler.Handle(id, request);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        return app;
    }
}