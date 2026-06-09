using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Core.Interfaces.ISMS.confKpi
{
    public interface IKpiField_conf
    {
        Task<KpiField_conf> CreateAsync(KpiField_conf kpiType);
        Task<KpiField_conf> UpdateAsync(KpiField_conf kpiType);
        Task<KpiField_conf?> DeleteAsync(string id);
        Task<List<KpiField_conf>> GetAllAsync();
        Task<KpiField_conf?> GetOneAsync(string id);
    }
}