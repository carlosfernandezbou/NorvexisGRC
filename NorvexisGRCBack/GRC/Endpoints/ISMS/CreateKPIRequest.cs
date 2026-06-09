namespace GRC.Endpoints.ISMS;

public record CreateKPIRequest(
    string KpiConfId,
    string KPICategoryId,
    string KpiName,
    string KPIResponsibleId,
    int? Value,
    int? TargetValue,
    string Comments,
    DateTime PeriodDate
);