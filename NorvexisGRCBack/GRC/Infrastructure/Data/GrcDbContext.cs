using Microsoft.EntityFrameworkCore;
using GRC.Core.Models.ISMS;
using GRC.Core.Models.SOA;
using GRC.Core.Models.RiskManagement;
using GRC.Core.Models.ISMS.confKpi;
using GRC.Infrastructure.Services;

using Audit.Core;
using Audit.EntityFramework;

namespace GRC.Infrastructure.Data;

public class GrcDbContext : AuditDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GrcDbContext(
        DbContextOptions<GrcDbContext> options,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<KPI> KPIs => Set<KPI>();
    public DbSet<KPICategory> KPICategories => Set<KPICategory>();
    public DbSet<KPIResponsible> KPIResponsibles => Set<KPIResponsible>();
    public DbSet<SOA> SOAs => Set<SOA>();
    public DbSet<RiskAssessment> RiskAssessments => Set<RiskAssessment>();
    public DbSet<RiskTreatment> RiskTreatments => Set<RiskTreatment>();
    public DbSet<RiskIdentification> RiskIdentifications => Set<RiskIdentification>();
    public DbSet<Kpi_conf> Kpi_confs { get; set; }
    public DbSet<KpiType_conf> KpiType_confs => Set<KpiType_conf>();
    public DbSet<KpiField_conf> KpiField_confs { get; set; }

    public override void OnScopeCreated(IAuditScope auditScope)
    {
        auditScope.SetCustomField("UserId", _currentUserService.UserId);
        auditScope.SetCustomField("Email", _currentUserService.Email);
        auditScope.SetCustomField("UserName", _currentUserService.UserName);

        var http = _httpContextAccessor.HttpContext;

        var endpoint = http?.GetEndpoint()?.DisplayName;
        var method = http?.Request?.Method;
        var path = http?.Request?.Path.Value;

        auditScope.EventType = endpoint ?? $"{method} {path}";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KPICategory>(entity =>
    {
        entity.ToContainer("KpiCategories");
        entity.HasKey(x => x.Id);
        entity.HasPartitionKey(x => x.Id);
        entity.HasNoDiscriminator();
    });

        modelBuilder.Entity<KPIResponsible>(entity =>
        {
            entity.ToContainer("KpiResponsibles");
            entity.HasKey(x => x.Id);
            entity.HasPartitionKey(x => x.Id);
            entity.HasNoDiscriminator();
        });

        modelBuilder.Entity<KPI>(entity =>
        {
            entity.ToContainer("Kpis");
            entity.HasKey(x => x.Id);
            entity.HasPartitionKey(x => x.Id);
            entity.HasNoDiscriminator();
        });

        modelBuilder.Entity<SOA>(entity =>
        {
            entity.ToContainer("SOAs");
            entity.HasKey(x => x.Id);
            entity.HasPartitionKey(x => x.Id);
            entity.HasNoDiscriminator();
        });

        modelBuilder.Entity<RiskAssessment>(entity =>
        {
            entity.ToContainer("RiskAssessments");
            entity.HasKey(x => x.Id);
            entity.HasPartitionKey(x => x.Id);
            entity.HasNoDiscriminator();
        });

        modelBuilder.Entity<RiskTreatment>(entity =>
        {
            entity.ToContainer("RiskTreatments");
            entity.HasKey(x => x.Id);
            entity.HasPartitionKey(x => x.Id);
            entity.HasNoDiscriminator();
        });

        modelBuilder.Entity<RiskIdentification>(entity =>
        {
            entity.ToContainer("RiskIdentifications");
            entity.HasKey(x => x.Id);
            entity.HasPartitionKey(x => x.Id);
            entity.HasNoDiscriminator();
        });

        modelBuilder.Entity<Kpi_conf>(entity =>
        {
            entity.ToContainer("Kpi_conf");
            entity.HasKey(x => x.Id);
            entity.HasPartitionKey(x => x.Id);
            entity.HasNoDiscriminator();
        });

        modelBuilder.Entity<KpiType_conf>(entity =>
        {
            entity.ToContainer("KpiType_conf");
            entity.HasKey(x => x.Id);
            entity.HasPartitionKey(x => x.Id);
            entity.HasNoDiscriminator();
        });

        modelBuilder.Entity<KpiField_conf>(entity =>
        {
            entity.ToContainer("KpiField_conf");
            entity.HasKey(x => x.Id);
            entity.HasPartitionKey(x => x.Id);
            entity.HasNoDiscriminator();
        });
    }
}