namespace GRC.Core.Models.ISMS;

public class KPICategory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string KPI_Category { get; set; } = string.Empty;
}