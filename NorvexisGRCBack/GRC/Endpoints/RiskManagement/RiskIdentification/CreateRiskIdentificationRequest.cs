namespace GRC.Endpoints.RiskManagement;

public record CreateRiskIdentificationRequest(
    string RiskId,
    string Category,
    string SubCategory,
    string AssetType,
    string AssetName,
    string Vulnerability,
    string Threats,
    string CoreValueImpacted
);