namespace GRC.Endpoints.ISMS.confKpi;

public record CreateKpiField_confRequest(
    string KpiConfId,
    string FieldName,
    string? DefaultValue
);