using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class DeleteKPICategoryHandler
{
    private readonly IKPICategoryService _kpiCategoryService;
    private readonly ILogger<DeleteKPICategoryHandler> _logger;

    public DeleteKPICategoryHandler(
        IKPICategoryService kpiCategoryService,
        ILogger<DeleteKPICategoryHandler> logger)
    {
        _kpiCategoryService = kpiCategoryService;
        _logger = logger;
    }

    public async Task<IResult> Handle(string id)
    {
        _logger.LogInformation("Deleting KPI Category {Id}", id);

        var category = await _kpiCategoryService.DeleteAsync(id);

        if (category is null)
            return Results.NotFound(new { error = "KPI Category not found" });

        return Results.Ok(category);
    }
}