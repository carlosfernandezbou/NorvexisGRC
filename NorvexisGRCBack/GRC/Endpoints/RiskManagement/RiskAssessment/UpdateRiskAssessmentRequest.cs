namespace GRC.Endpoints.RiskManagement;

public record UpdateRiskAssessmentRequest(
    string RiskId,
    string RiskImpactDescription,
    string ExistingControls,
    string Impact,
    string Likelihood,
    string RiskCategory
);