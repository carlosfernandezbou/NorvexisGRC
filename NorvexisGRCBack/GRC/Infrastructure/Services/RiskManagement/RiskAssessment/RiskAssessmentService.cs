using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Services;

public class RiskAssessmentService : IRiskAssessmentService
{
    private readonly GrcDbContext _context;

    public RiskAssessmentService(GrcDbContext context)
    {
        _context = context;
    }

    public async Task<List<RiskAssessment>> GetAllAsync()
    {
        return await _context.RiskAssessments.ToListAsync();
    }

    public async Task<RiskAssessment?> GetOneAsync(string id)
    {
        return await _context.RiskAssessments.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RiskAssessment> CreateAsync(
        string riskId,
        string riskImpactDescription,
        string existingControls,
        string impact,
        string likelihood,
        string riskCategory)
    {
        var riskAssessment = new RiskAssessment
        {
            RiskId = riskId,
            RiskImpactDescription = riskImpactDescription,
            ExistingControls = existingControls,
            Impact = impact,
            Likelihood = likelihood,
            RiskCategory = riskCategory
        };

        _context.RiskAssessments.Add(riskAssessment);
        await _context.SaveChangesAsync();

        return riskAssessment;
    }

    public async Task<RiskAssessment> CreateAsync(RiskAssessment riskAssessment)
    {
        _context.RiskAssessments.Add(riskAssessment);
        await _context.SaveChangesAsync();

        return riskAssessment;
    }

    public async Task<RiskAssessment?> UpdateAsync(
        string id,
        string riskId,
        string riskImpactDescription,
        string existingControls,
        string impact,
        string likelihood,
        string riskCategory)
    {
        var riskAssessment = await _context.RiskAssessments.FirstOrDefaultAsync(r => r.Id == id);

        if (riskAssessment == null)
        {
            return null;
        }

        riskAssessment.RiskId = riskId;
        riskAssessment.RiskImpactDescription = riskImpactDescription;
        riskAssessment.ExistingControls = existingControls;
        riskAssessment.Impact = impact;
        riskAssessment.Likelihood = likelihood;
        riskAssessment.RiskCategory = riskCategory;

        await _context.SaveChangesAsync();

        return riskAssessment;
    }

    public async Task<RiskAssessment> UpdateAsync(RiskAssessment riskAssessment)
    {
        var existingRiskAssessment = await _context.RiskAssessments.FirstOrDefaultAsync(r => r.Id == riskAssessment.Id);
        if (existingRiskAssessment == null)
        {
            throw new Exception("Risk Assessment not found");
        }

        _context.Entry(existingRiskAssessment).CurrentValues.SetValues(riskAssessment);
        await _context.SaveChangesAsync();

        return existingRiskAssessment;
    }

    public async Task<RiskAssessment?> DeleteAsync(string id)
    {
        var riskAssessment = await _context.RiskAssessments.FirstOrDefaultAsync(r => r.Id == id);

        if (riskAssessment == null)
        {
            return null;
        }

        _context.RiskAssessments.Remove(riskAssessment);
        await _context.SaveChangesAsync();

        return riskAssessment;
    }
}