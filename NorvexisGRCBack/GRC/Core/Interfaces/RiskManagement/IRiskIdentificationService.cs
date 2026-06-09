using GRC.Core.Models.RiskManagement;

namespace GRC.Core.Interfaces.RiskManagement
{
    public interface IRiskIdentificationService
    {
        Task<List<RiskIdentification>> GetAllAsync();
        Task<RiskIdentification?> GetOneAsync(string id);
        Task<RiskIdentification> CreateAsync(string riskId, string category, string subCategory, string assetType, string assetName, string vulnerability, string threats, string coreValueImpacted);
        Task<RiskIdentification> CreateAsync(RiskIdentification riskIdentification);
        Task<RiskIdentification?> DeleteAsync(string id);
        Task<RiskIdentification?> UpdateAsync (string id, string riskId, string category, string subCategory, string assetType, string assetName, string vulnerability, string threats, string coreValueImpacted);
        Task<RiskIdentification> UpdateAsync(RiskIdentification riskIdentification);
    }
}