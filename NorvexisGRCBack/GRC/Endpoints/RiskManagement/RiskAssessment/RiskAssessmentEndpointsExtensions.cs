using GRC.Core.Interfaces.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;

namespace GRC.Endpoints.RiskManagement;

public static class RiskAssessmentEndpointsExtensions
{
    public static IEndpointRouteBuilder MapRiskAssessmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/risk-assessments");

        group.MapGet("/", async (IRiskAssessmentService riskAssessmentService) =>
        {
            var riskAssessments = await riskAssessmentService.GetAllAsync();
            return Results.Ok(riskAssessments);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapGet("/{id}", async (string id, IRiskAssessmentService riskAssessmentService) =>
        {
            var riskAssessment = await riskAssessmentService.GetOneAsync(id);

            if (riskAssessment == null)
                return Results.NotFound(new { error = "RiskAssessment not found" });

            return Results.Ok(riskAssessment);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPost("/", async (CreateRiskAssessmentRequest request, CreateRiskAssessmentHandler handler) =>
        {
            return await handler.Handle(request);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapDelete("/{id}", async (string id, DeleteRiskAssessmentHandler handler) =>
        {
            return await handler.Handle(id);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        group.MapPut("/{id}", async (string id, UpdateRiskAssessmentRequest request, UpdateRiskAssessmentHandler handler) =>
        {
            return await handler.Handle(id, request);
        }).WithTags("RiskManagement").RequireAuthorization(policy => policy.RequireRole("Admin", "NormalUser"));

        return app;
    }
}