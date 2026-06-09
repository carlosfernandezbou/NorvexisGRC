using GRC.Core.Interfaces.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;

namespace GRC.Endpoints.RiskManagement;

public static class RiskIdentificationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapRiskIdentificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/risk-identifications");

        group.MapGet("/", async (IRiskIdentificationService riskIdentificationService) =>
        {
            var riskIdentifications = await riskIdentificationService.GetAllAsync();
            return Results.Ok(riskIdentifications);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapGet("/{id}", async (string id, IRiskIdentificationService riskIdentificationService) =>
        {
            var riskIdentification = await riskIdentificationService.GetOneAsync(id);

            if (riskIdentification == null)
                return Results.NotFound(new { error = "RiskIdentification not found" });

            return Results.Ok(riskIdentification);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/", async (CreateRiskIdentificationRequest request, CreateRiskIdentificationHandler handler) =>
        {
            return await handler.Handle(request);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/{id}", async (string id, DeleteRiskIdentificationHandler handler) =>
        {
            return await handler.Handle(id);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPut("/{id}", async (string id, UpdateRiskIdentificationRequest request, UpdateRiskIdentificationHandler handler) =>
        {
            return await handler.Handle(id, request);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        return app;
    }
}