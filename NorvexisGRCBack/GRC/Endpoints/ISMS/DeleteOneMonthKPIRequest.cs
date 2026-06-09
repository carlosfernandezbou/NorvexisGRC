namespace GRC.Endpoints.ISMS;

public record DeleteOneMonthKPIRequest(
    int Year,
    int Month
);