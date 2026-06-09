namespace GRC.Core.Models.RiskManagement
{
    public class RiskAssessment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RiskId { get; set; } = string.Empty;
        public string RiskImpactDescription { get; set; } = string.Empty;
        public string ExistingControls { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public string Likelihood { get; set; } = string.Empty;
        public string RiskCategory { get; set; } = string.Empty;
    }
}