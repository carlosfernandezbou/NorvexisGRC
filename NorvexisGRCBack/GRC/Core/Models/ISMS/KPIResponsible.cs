namespace GRC.Core.Models.ISMS;

public class KPIResponsible
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Responsible { get; set; } = string.Empty;
}