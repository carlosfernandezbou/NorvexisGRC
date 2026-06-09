using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class UpdateKPICategoryHandler
{
    private readonly IKPICategoryService _kpiCategoryService;
    private readonly ILogger<UpdateKPICategoryHandler> _logger;

    public UpdateKPICategoryHandler(
        IKPICategoryService kpiCategoryService,
        ILogger<UpdateKPICategoryHandler> logger)
    {
        _kpiCategoryService = kpiCategoryService;
        _logger = logger;
    }

    public async Task<IResult> Handle(string id, UpdateKPICategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.KPICategory))
        {
            return Results.BadRequest(new { error = "KPICategory is required" });
        }

        _logger.LogInformation("Updating KPI Category {Id}", id);

        KPICategory category = new KPICategory
        {
            Id = id,
            KPI_Category = request.KPICategory,
        };

        category = await _kpiCategoryService.UpdateAsync(category);

        // var category = await _kpiCategoryService.UpdateAsync(id, request.KPICategory);

        if (category is null)
            return Results.NotFound(new { error = "KPI Category not found" });

        return Results.Ok(category);
    }
}