using LinqToDB.Data;
using HistoryTracking.Audit.Configuration;
using HistoryTracking.Audit.Repositories;
using HistoryTracking.Audit.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HistoryTracking.Audit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudit(
        this IServiceCollection services,
        Action<IAuditModelBuilder> configure = null)
    {
        var modelBuilder = new AuditModelBuilder();
        configure?.Invoke(modelBuilder);
        services.AddSingleton(modelBuilder.Build());

        services.AddScoped<IHistoryContext, HistoryContext>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        return services;
    }
}
