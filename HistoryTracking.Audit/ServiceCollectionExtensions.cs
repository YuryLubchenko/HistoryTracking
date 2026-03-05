using HistoryTracking.Audit.Configuration;
using HistoryTracking.Audit.Repositories;
using HistoryTracking.Audit.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HistoryTracking.Audit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudit(
        this IServiceCollection services,
        Action<AuditOptions> configureOptions,
        Action<IAuditModelBuilder> configure = null)
        => AddAuditCore(services, configurationSection: null, configureOptions, configure);

    public static IServiceCollection AddAudit(
        this IServiceCollection services,
        IConfigurationSection configurationSection,
        Action<AuditOptions> configureOptions = null,
        Action<IAuditModelBuilder> configure = null)
        => AddAuditCore(services, configurationSection, configureOptions, configure);

    private static IServiceCollection AddAuditCore(
        IServiceCollection services,
        IConfigurationSection configurationSection,
        Action<AuditOptions> configureOptions,
        Action<IAuditModelBuilder> configure)
    {
        if (configurationSection != null)
            services.Configure<AuditOptions>(options => configurationSection.Bind(options));

        services.Configure<AuditOptions>(options => configureOptions?.Invoke(options));

        var startupOptions = new AuditOptions();
        configurationSection?.Bind(startupOptions);
        configureOptions?.Invoke(startupOptions);
        var schemaName = startupOptions.DatabaseSchemaName;

        var modelBuilder = new AuditModelBuilder();
        configure?.Invoke(modelBuilder);
        services.AddSingleton(modelBuilder.Build(schemaName));

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditScopeFactory, AuditScopeFactory>();
        services.AddScoped<IAuditWriterService, AuditWriterService>();

        return services;
    }
}
