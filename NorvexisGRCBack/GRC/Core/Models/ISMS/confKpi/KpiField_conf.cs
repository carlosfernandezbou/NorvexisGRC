namespace GRC.Core.Models.ISMS.confKpi;

public class KpiField_conf
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string KpiConfId { get; set; } = string.Empty;

    public string FieldName { get; set; } = string.Empty;
    public string? DefaultValue { get; set; }
}
