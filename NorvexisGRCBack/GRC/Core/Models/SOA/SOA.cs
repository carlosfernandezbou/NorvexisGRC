namespace GRC.Core.Models.SOA;

public class SOA
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Section { get; set; } = string.Empty;
    public string Control { get; set; } = string.Empty;
    public string Tittle { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public bool Applicable { get; set; } = false;
    public bool Implemented { get; set; } = false;
    public string Justification { get; set; } = string.Empty;
    public string Evidence { get; set; } = string.Empty;
}