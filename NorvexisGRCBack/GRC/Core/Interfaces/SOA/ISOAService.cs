using SOAModel = GRC.Core.Models.SOA.SOA;

namespace GRC.Core.Interfaces.SOA
{
    public interface ISOAService
    {
        Task<List<SOAModel>> GetAllAsync();
        Task<SOAModel?> GetOneAsync(string id);
        Task<SOAModel> CreateAsync(string section, string control, string tittle, string objective, bool applicable, bool implemented, string justification, string evidence);
        Task<SOAModel> CreateAsync(SOAModel soa);
        Task<SOAModel?> DeleteAsync(string id);
        Task<SOAModel?> UpdateAsync(string id, string section, string control, string tittle, string objective, bool applicable, bool implemented, string justification, string evidence);
        Task<SOAModel> UpdateAsync(SOAModel soa);
        Task<int> ImportFromJsonAsync(Stream jsonStream);
    }
}