namespace GRC.Core.Models.ISMS.confKpi;

public class KpiType_conf
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty;
}
