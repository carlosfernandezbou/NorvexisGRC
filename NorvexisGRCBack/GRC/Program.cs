using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AspNetCore.Identity.CosmosDb;
using AspNetCore.Identity.CosmosDb.Extensions;
using Microsoft.AspNetCore.Identity;
using Quartz;
using Audit.Core;
using Microsoft.Azure.Cosmos;
using OpenTelemetry.Metrics;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using UserModel = GRC.Core.Models.Users.User;
using GRC.Core.Interfaces.RiskManagement;
using GRC.Core.Interfaces.ISMS;
using GRC.Core.Interfaces.SOA;
using GRC.Core.Interfaces.ISMS.confKpi;

using GRC.Endpoints.ISMS;
using GRC.Endpoints.ISMS.Handlers;
using GRC.Endpoints.ISMS.confKpi;
using GRC.Endpoints.ISMS.confKpi.Handlers;
using GRC.Endpoints;
using GRC.Endpoints.SOA;
using GRC.Endpoints.SOA.Handlers;
using GRC.Endpoints.RiskManagement;
using GRC.Endpoints.RiskManagement.Handlers;
using GRC.Endpoints.Users;
using GRC.Endpoints.Auth;

using GRC.Infrastructure.Data;
using GRC.Infrastructure.Services;
using GRC.Infrastructure.Jobs;
using GRC.Infrastructure.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
//

// COSMOSDB
var endpoint = builder.Configuration["CosmosDb:AccountEndpoint"]
    ?? throw new InvalidOperationException("Missing config: CosmosDb:AccountEndpoint");

var key = builder.Configuration["CosmosDb:AccountKey"]
    ?? throw new InvalidOperationException("Missing config: CosmosDb:AccountKey");

var databaseName = builder.Configuration["CosmosDb:DatabaseName"]
    ?? throw new InvalidOperationException("Missing config: CosmosDb:DatabaseName");

builder.Services.AddDbContext<GrcDbContext>(options =>
    options.UseCosmos(
        endpoint,
        key,
        databaseName));
//

//AUDIT TRAIL
Audit.Core.Configuration.Setup()
    .UseAzureCosmos(config => config
        .Endpoint(endpoint)
        .AuthKey(key)
        .Database(databaseName)
        .Container("AuditEvents")
    );
//

// IDENTITY WITH COSMOS
builder.Services.AddDbContext<IdentityCosmosDbContext>(options =>
    options.UseCosmos(endpoint, key, databaseName));

builder.Services
    .AddCosmosIdentity<IdentityCosmosDbContext, UserModel, IdentityRole, string>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        // LOCKOUT
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Missing config: Jwt:Key");
//

// Login
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
//

// ISMS
builder.Services.AddScoped<IKPIService, KpiService>();
builder.Services.AddScoped<IKPICategoryService, KPICategoryService>();
builder.Services.AddScoped<IKPIResponsibleService, KPIResponsibleService>();

builder.Services.AddScoped<CreateKPIHandler>();
builder.Services.AddScoped<UpdateKPIHandler>();
builder.Services.AddScoped<DeleteKPIHandler>();
builder.Services.AddScoped<DeleteOneMonthKPIHandler>();
builder.Services.AddScoped<GenerateKpisHandler>();

builder.Services.AddScoped<CreateKPICategoryHandler>();
builder.Services.AddScoped<UpdateKPICategoryHandler>();
builder.Services.AddScoped<DeleteKPICategoryHandler>();

builder.Services.AddScoped<CreateKPIResponsibleHandler>();
builder.Services.AddScoped<UpdateKPIResponsibleHandler>();
builder.Services.AddScoped<DeleteKPIResponsibleHandler>();
//

// KPIs configuration
builder.Services.AddScoped<IKpi_conf, Kpi_confService>();
builder.Services.AddScoped<CreateKpi_confHandler>();
builder.Services.AddScoped<UpdateKpi_confHandler>();
builder.Services.AddScoped<DeleteKpi_confHandler>();

builder.Services.AddScoped<IKpiType_conf, KpiType_confService>();
builder.Services.AddScoped<CreateKpiType_confHandler>();
builder.Services.AddScoped<UpdateKpiType_confHandler>();
builder.Services.AddScoped<DeleteKpiType_confHandler>();

builder.Services.AddScoped<IKpiField_conf, KpiField_confService>();
builder.Services.AddScoped<CreateKpiField_confHandler>();
builder.Services.AddScoped<UpdateKpiField_confHandler>();
builder.Services.AddScoped<DeleteKpiField_confHandler>();

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("MonthlyKpiJob");

    q.AddJob<MonthlyKpiJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("MonthlyKpiJob-trigger")
         //.WithCronSchedule("0 */1 * * * ?")); // Ejecutar cada minuto para pruebas. Cambiar a "0 0 0 1 * ?"  para ejecutar el primer día de cada mes o "0 5 0 * * ?" cada dia
         .WithCronSchedule("0 5 0 * * ?"));


    /*   var resetDbJobKey = new JobKey("ResetPortfolioDbJob");

         q.AddJob<ResetPortfolioDbJob>(opts => opts.WithIdentity(resetDbJobKey));

         q.AddTrigger(opts => opts
             .ForJob(resetDbJobKey)
             .WithIdentity("ResetPortfolioDbJob-trigger")
             .WithCronSchedule("0 0/30 * * * ?")); // cada 30 minutos
    */
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});
//

// SOA
builder.Services.AddScoped<ISOAService, SoaService>();
builder.Services.AddScoped<CreateSOAHandler>();
builder.Services.AddScoped<UpdateSOAHandler>();
builder.Services.AddScoped<DeleteSOAHandler>();
//

// RISK MANAGEMENT
builder.Services.AddScoped<IRiskAssessmentService, RiskAssessmentService>();
builder.Services.AddScoped<CreateRiskAssessmentHandler>();
builder.Services.AddScoped<UpdateRiskAssessmentHandler>();
builder.Services.AddScoped<DeleteRiskAssessmentHandler>();

builder.Services.AddScoped<IRiskTreatmentService, RiskTreatmentService>();
builder.Services.AddScoped<CreateRiskTreatmentHandler>();
builder.Services.AddScoped<UpdateRiskTreatmentHandler>();
builder.Services.AddScoped<DeleteRiskTreatmentHandler>();

builder.Services.AddScoped<IRiskIdentificationService, RiskIdentificationService>();
builder.Services.AddScoped<CreateRiskIdentificationHandler>();
builder.Services.AddScoped<UpdateRiskIdentificationHandler>();
builder.Services.AddScoped<DeleteRiskIdentificationHandler>();
//

builder.Services.AddApplicationServices();

//Swagger + Login
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
//

// METRICS
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeter("GRC.Business")
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });
//

// RATE LIMITING
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 500,
                Window = TimeSpan.FromMinutes(10),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
//

var app = builder.Build();

//CREATE DB AND SEED
try
{
    await CosmosInitializer.InitializeAsync(app.Services, endpoint, key, databaseName);
    await UserSeeder.SeedAsync(app.Services);
    await KpiConfSeeder.SeedAsync(app.Services);
    await SoaSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Cosmos DB no disponible durante el arranque");
}
//

app.UseSwagger();
app.UseSwaggerUI();

// CORS ANGULAR
app.UseCors("AllowAngular");
//

// Identity
app.UseAuthentication();
app.UseAuthorization();

// RATE LIMITING
app.UseRateLimiter();
//

// Auth
app.MapAuthEndpoints();

// Metrics
app.MapPrometheusScrapingEndpoint();

//Health
app.MapHealthEndpoints();

// ISMS
app.MapKPIEndpoints();
app.MapKPICategoryEndpoints();
app.MapKPIResponsibleEndpoints();

// KPIs configuration
app.MapKpi_confEndpoints();
app.MapKpiType_confEndpoints();
app.MapKpiField_confEndpoints();

//SOA
app.MapSOAEndpoints();

// RISK MANAGEMENT
app.MapRiskAssessmentEndpoints();
app.MapRiskTreatmentEndpoints();
app.MapRiskIdentificationEndpoints();

// User
app.MapUserEndpoints();

app.Run();

public partial class Program { }