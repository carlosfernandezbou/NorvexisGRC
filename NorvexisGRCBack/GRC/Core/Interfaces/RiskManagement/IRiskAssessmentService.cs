using GRC.Core.Models.RiskManagement;

namespace GRC.Core.Interfaces.RiskManagement
{
    public interface IRiskAssessmentService
    {
        Task<List<RiskAssessment>> GetAllAsync();
        Task<RiskAssessment?> GetOneAsync(string id);
        Task<RiskAssessment> CreateAsync(string riskId, string riskImpactDescription, string existingControls, string impact, string likelihood, string riskCategory);
        Task<RiskAssessment> CreateAsync(RiskAssessment riskAssessment);
        Task<RiskAssessment?> DeleteAsync(string id);
        Task<RiskAssessment?> UpdateAsync (string id, string riskId, string riskImpactDescription, string existingControls, string impact, string likelihood, string riskCategory);    
        Task<RiskAssessment> UpdateAsync(RiskAssessment riskAssessment);

        }
}