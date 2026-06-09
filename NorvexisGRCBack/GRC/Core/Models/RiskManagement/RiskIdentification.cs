namespace GRC.Core.Models.RiskManagement
{
    public class RiskIdentification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RiskId { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        public string AssetName  { get; set; } = string.Empty;
        public string Vulnerability { get; set; } = string.Empty;
        public string Threats { get; set; } = string.Empty;
        public string CoreValueImpacted { get; set; } = string.Empty;
    }
}