using LinqToDB.Data;
using HistoryTracking.Audit.Configuration;
using HistoryTracking.Audit.Repositories;
using HistoryTracking.Audit.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HistoryTracking.Audit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudit(
        this IServiceCollection services,
        Action<AuditOptions> configureOptions,
        Action<IAuditModelBuilder> configure = null)
    {
        services.Configure<AuditOptions>(options => configureOptions?.Invoke(options));

        var modelBuilder = new AuditModelBuilder();
        configure?.Invoke(modelBuilder);
        services.AddSingleton(modelBuilder.Build());

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditScopeFactory, HistoryContext>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        return services;
    }
}
