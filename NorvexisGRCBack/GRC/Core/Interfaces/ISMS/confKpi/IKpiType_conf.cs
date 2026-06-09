using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Core.Interfaces.ISMS.confKpi
{
    public interface IKpiType_conf
    {
        Task<KpiType_conf> CreateAsync(KpiType_conf kpiType);
        Task<KpiType_conf> UpdateAsync(KpiType_conf kpiType);
        Task<KpiType_conf?> DeleteAsync(string id);
        Task<List<KpiType_conf>> GetAllAsync();
        Task<KpiType_conf?> GetOneAsync(string id);
    }
}