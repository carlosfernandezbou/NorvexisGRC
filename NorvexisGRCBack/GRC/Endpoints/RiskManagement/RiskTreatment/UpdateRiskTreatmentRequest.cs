namespace GRC.Endpoints.RiskManagement;

public record UpdateRiskTreatmentRequest(
    string RiskId,
    string RiskTreatmentOption,
    string TreatmentPlan,
    string Owner,
    string DueDate,
    string Status,
    string ResidualRisk,
    string Comments
);