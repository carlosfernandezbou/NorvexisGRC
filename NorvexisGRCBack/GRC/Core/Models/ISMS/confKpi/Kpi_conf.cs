namespace GRC.Core.Models.ISMS.confKpi;

public class Kpi_conf
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string KpiTypeConfId { get; set; } = string.Empty; // Monthly, Weekly...
}