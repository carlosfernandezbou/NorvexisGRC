namespace GRC.Endpoints.RiskManagement;

public record CreateRiskAssessmentRequest(
    string RiskId,
    string RiskImpactDescription,
    string ExistingControls,
    string Impact,
    string Likelihood,
    string RiskCategory
);