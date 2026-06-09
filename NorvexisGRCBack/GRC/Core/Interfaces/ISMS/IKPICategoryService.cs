using GRC.Core.Models.ISMS;

namespace GRC.Core.Interfaces.ISMS;

public interface IKPICategoryService
{
    Task<KPICategory> CreateAsync(string kpiCategory);
    Task<KPICategory> CreateAsync(KPICategory kpiCategory);
    Task<KPICategory?> UpdateAsync(string id, string kpiCategory);
    Task<KPICategory> UpdateAsync(KPICategory kpiCategory);
    Task<KPICategory?> DeleteAsync(string id);
    Task<List<KPICategory>> GetAllAsync();
    Task<KPICategory?> GetOneAsync(string id);
}