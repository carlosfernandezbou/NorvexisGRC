using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Services;

public class RiskIdentificationService : IRiskIdentificationService
{
    private readonly GrcDbContext _context;

    public RiskIdentificationService(GrcDbContext context)
    {
        _context = context;
    }

    public async Task<List<RiskIdentification>> GetAllAsync()
    {
        return await _context.RiskIdentifications.ToListAsync();
    }

    public async Task<RiskIdentification?> GetOneAsync(string id)
    {
        return await _context.RiskIdentifications.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RiskIdentification> CreateAsync(
        string riskId,
        string category,
        string subCategory,
        string assetType,
        string assetName,
        string vulnerability,
        string threats,
        string coreValueImpacted)
    {
        var riskIdentification = new RiskIdentification
        {
            RiskId = riskId,
            Category = category,
            SubCategory = subCategory,
            AssetType = assetType,
            AssetName = assetName,
            Vulnerability = vulnerability,
            Threats = threats,
            CoreValueImpacted = coreValueImpacted
        };

        _context.RiskIdentifications.Add(riskIdentification);
        await _context.SaveChangesAsync();

        return riskIdentification;
    }

    public async Task<RiskIdentification> CreateAsync(RiskIdentification riskIdentification)
    {
        _context.RiskIdentifications.Add(riskIdentification);
        await _context.SaveChangesAsync();

        return riskIdentification;
    }

    public async Task<RiskIdentification?> UpdateAsync(
        string id,
        string riskId,
        string category,
        string subCategory,
        string assetType,
        string assetName,
        string vulnerability,
        string threats,
        string coreValueImpacted)
    {
        var riskIdentification = await _context.RiskIdentifications.FirstOrDefaultAsync(r => r.Id == id);

        if (riskIdentification == null)
        {
            return null;
        }

        riskIdentification.RiskId = riskId;
        riskIdentification.Category = category;
        riskIdentification.SubCategory = subCategory;
        riskIdentification.AssetType = assetType;
        riskIdentification.AssetName = assetName;
        riskIdentification.Vulnerability = vulnerability;
        riskIdentification.Threats = threats;
        riskIdentification.CoreValueImpacted = coreValueImpacted;

        await _context.SaveChangesAsync();

        return riskIdentification;
    }

    public async Task<RiskIdentification> UpdateAsync(RiskIdentification riskIdentification)
    {
        var existingRiskIdentification = await _context.RiskIdentifications.FirstOrDefaultAsync(r => r.Id == riskIdentification.Id);
        if (existingRiskIdentification == null)
        {
            throw new Exception("Risk Identification not found");
        }

        _context.Entry(existingRiskIdentification).CurrentValues.SetValues(riskIdentification);
        await _context.SaveChangesAsync();

        return existingRiskIdentification;
    }

    public async Task<RiskIdentification?> DeleteAsync(string id)
    {
        var riskIdentification = await _context.RiskIdentifications.FirstOrDefaultAsync(r => r.Id == id);

        if (riskIdentification == null)
        {
            return null;
        }

        _context.RiskIdentifications.Remove(riskIdentification);
        await _context.SaveChangesAsync();

        return riskIdentification;
    }
}