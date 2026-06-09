namespace GRC.Endpoints.ISMS.confKpi;

public record UpdateKpiField_confRequest(
    string KpiConfId,
    string FieldName,
    string? DefaultValue
);