namespace GRC.Endpoints.RiskManagement;

public record CreateRiskTreatmentRequest(
    string RiskId,
    string RiskTreatmentOption,
    string TreatmentPlan,
    string Owner,
    string DueDate,
    string Status,
    string ResidualRisk,
    string Comments
);