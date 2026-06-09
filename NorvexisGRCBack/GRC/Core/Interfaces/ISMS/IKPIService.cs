using GRC.Core.Models.ISMS;

namespace GRC.Core.Interfaces.ISMS
{
    public interface IKPIService
    {
        Task<KPI> CreateAsync(string kpiCategoryId, string kpiName, string kpiResponsibleId, int value, string comments);
        Task<KPI> CreateAsync(KPI kpi);
        Task<KPI?> UpdateAsync(string id, string kpiCategoryId, string kpiName, string kpiResponsibleId, int value, string comments);
        Task<KPI> UpdateAsync(KPI kpi);
        Task<KPI?> DeleteAsync(string id);
        Task<int> DeleteByMonthAsync(int year, int month);
        Task<List<KPI>> GetAllAsync();
        Task<KPI?> GetOneAsync(string id);
        Task GenerateKpisAsync();
        Task GenerateKpisForDateAsync(DateTime date);
    }
}
