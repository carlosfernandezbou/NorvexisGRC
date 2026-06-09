namespace GRC.Core.Models.RiskManagement
{
    public class RiskTreatment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RiskId { get; set; } = string.Empty;
        public string RiskTreatmentOption { get; set; } = string.Empty;
        public string TreatmentPlan { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ResidualRisk { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
    }
}