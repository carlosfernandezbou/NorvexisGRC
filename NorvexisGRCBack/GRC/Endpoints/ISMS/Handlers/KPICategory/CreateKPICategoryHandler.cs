using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Models.ISMS;

namespace GRC.Endpoints.ISMS.Handlers;

public class CreateKPICategoryHandler
{
    private readonly IKPICategoryService _kpiCategoryService;
    private readonly ILogger<CreateKPICategoryHandler> _logger;

    public CreateKPICategoryHandler(
        IKPICategoryService kpiCategoryService,
        ILogger<CreateKPICategoryHandler> logger)
    {
        _kpiCategoryService = kpiCategoryService;
        _logger = logger;
    }

    public async Task<IResult> Handle(CreateKPICategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.KPICategory))
        {
            return Results.BadRequest(new { error = "KPICategory is required" });
        }

        _logger.LogInformation("Creating KPI Category {KPICategory}", request.KPICategory);

        KPICategory kpiCategory = new KPICategory
        {
            KPI_Category = request.KPICategory
        };

        kpiCategory = await _kpiCategoryService.CreateAsync(kpiCategory);

        // var category = await _kpiCategoryService.CreateAsync(request.KPICategory);

        return Results.Ok(kpiCategory);
    }
}