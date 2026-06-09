namespace GRC.Endpoints.ISMS;

public record UpdateKPIRequest(
    string KPICategoryId,
    string KpiName,
    string KPIResponsibleId,
    int? Value,
    int? TargetValue,
    string Comments
);