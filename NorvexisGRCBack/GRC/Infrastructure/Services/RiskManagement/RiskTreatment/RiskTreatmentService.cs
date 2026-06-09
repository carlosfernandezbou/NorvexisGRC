using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Models.RiskManagement;
using GRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Services;

public class RiskTreatmentService : IRiskTreatmentService
{
    private readonly GrcDbContext _context;

    public RiskTreatmentService(GrcDbContext context)
    {
        _context = context;
    }

    public async Task<List<RiskTreatment>> GetAllAsync()
    {
        return await _context.RiskTreatments.ToListAsync();
    }

    public async Task<RiskTreatment?> GetOneAsync(string id)
    {
        return await _context.RiskTreatments.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RiskTreatment> CreateAsync(
        string riskId,
        string riskTreatmentOption,
        string treatmentPlan,
        string owner,
        string dueDate,
        string status,
        string residualRisk,
        string comments)
    {
        var riskTreatment = new RiskTreatment
        {
            RiskId = riskId,
            RiskTreatmentOption = riskTreatmentOption,
            TreatmentPlan = treatmentPlan,
            Owner = owner,
            DueDate = dueDate,
            Status = status,
            ResidualRisk = residualRisk,
            Comments = comments
        };

        _context.RiskTreatments.Add(riskTreatment);
        await _context.SaveChangesAsync();

        return riskTreatment;
    }

    public async Task<RiskTreatment> CreateAsync(RiskTreatment riskTreatment)
    {
        _context.RiskTreatments.Add(riskTreatment);
        await _context.SaveChangesAsync();

        return riskTreatment;
    }

    public async Task<RiskTreatment?> UpdateAsync(
        string id,
        string riskId,
        string riskTreatmentOption,
        string treatmentPlan,
        string owner,
        string dueDate,
        string status,
        string residualRisk,
        string comments)
    {
        var riskTreatment = await _context.RiskTreatments.FirstOrDefaultAsync(r => r.Id == id);

        if (riskTreatment == null)
        {
            return null;
        }

        riskTreatment.RiskId = riskId;
        riskTreatment.RiskTreatmentOption = riskTreatmentOption;
        riskTreatment.TreatmentPlan = treatmentPlan;
        riskTreatment.Owner = owner;
        riskTreatment.DueDate = dueDate;
        riskTreatment.Status = status;
        riskTreatment.ResidualRisk = residualRisk;
        riskTreatment.Comments = comments;

        await _context.SaveChangesAsync();

        return riskTreatment;
    }

    public async Task<RiskTreatment> UpdateAsync(RiskTreatment riskTreatment)
    {
        var existingRiskTreatment = await _context.RiskTreatments.FirstOrDefaultAsync(r => r.Id == riskTreatment.Id);
        if (existingRiskTreatment == null)
        {
            throw new Exception("Risk Treatment not found");
        }

        _context.Entry(existingRiskTreatment).CurrentValues.SetValues(riskTreatment);
        await _context.SaveChangesAsync();

        return existingRiskTreatment;
    }

    public async Task<RiskTreatment?> DeleteAsync(string id)
    {
        var riskTreatment = await _context.RiskTreatments.FirstOrDefaultAsync(r => r.Id == id);

        if (riskTreatment == null)
        {
            return null;
        }

        _context.RiskTreatments.Remove(riskTreatment);
        await _context.SaveChangesAsync();

        return riskTreatment;
    }
}