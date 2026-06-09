using GRC.Core.Models.RiskManagement;

namespace GRC.Core.Interfaces.RiskManagement
{
    public interface IRiskTreatmentService
    {
        Task<List<RiskTreatment>> GetAllAsync();
        Task<RiskTreatment?> GetOneAsync(string id);
        Task<RiskTreatment> CreateAsync(string riskId, string riskTreatmentOption, string treatmentPlan, string owner, string dueDate, string status, string residualRisk, string comments);
        Task<RiskTreatment> CreateAsync(RiskTreatment riskTreatment);
        Task<RiskTreatment?> DeleteAsync(string id);
        Task<RiskTreatment?> UpdateAsync (string id, string riskId, string riskTreatmentOption, string treatmentPlan, string owner, string dueDate, string status, string residualRisk, string comments);    
        Task<RiskTreatment> UpdateAsync(RiskTreatment riskTreatment);
        }
}