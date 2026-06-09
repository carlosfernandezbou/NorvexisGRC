namespace GRC.Core.Models.ISMS;

public class KPI
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Referencia a la plantilla/configuración que lo generó
    public string KpiConfId { get; set; } = string.Empty;

    // Campos que se rellenan desde KpiField_conf
    public string KPI_Name { get; set; } = string.Empty;
    public string KPICategoryId { get; set; } = string.Empty;
    public string KPIResponsibleId { get; set; } = string.Empty;

    public int? Value { get; set; }
    public int? TargetValue { get; set; }
    public string Comments { get; set; } = string.Empty;

    // Mes/año al que pertenece este KPI generado
    public DateTime PeriodDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string PeriodKey { get; set; } = string.Empty;
    public bool ManuallyCreate { get; set; } = true;
}