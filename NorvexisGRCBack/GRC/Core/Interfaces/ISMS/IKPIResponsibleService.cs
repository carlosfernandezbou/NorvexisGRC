using GRC.Core.Models.ISMS;

namespace GRC.Core.Interfaces.ISMS;

public interface IKPIResponsibleService
{
    Task<KPIResponsible> CreateAsync(string responsible);
    Task<KPIResponsible> CreateAsync(KPIResponsible kpiResponsible);
    Task<KPIResponsible?> UpdateAsync(string id, string responsible);
    Task<KPIResponsible> UpdateAsync(KPIResponsible kpiResponsible);
    Task<KPIResponsible?> DeleteAsync(string id);
    Task<List<KPIResponsible>> GetAllAsync();
    Task<KPIResponsible?> GetOneAsync(string id);
}