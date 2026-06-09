using GRC.Core.Models.ISMS.confKpi;

namespace GRC.Core.Interfaces.ISMS.confKpi
{
    public interface IKpi_conf
    {
        Task<Kpi_conf> CreateAsync(Kpi_conf kpiType);
        Task<Kpi_conf> UpdateAsync(Kpi_conf kpiType);
        Task<Kpi_conf?> DeleteAsync(string id);
        Task<List<Kpi_conf>> GetAllAsync();
        Task<Kpi_conf?> GetOneAsync(string id);
    }
}