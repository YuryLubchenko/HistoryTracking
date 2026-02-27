using System.Reflection;

namespace HistoryTracking.Audit.Configuration;

internal class AuditModelBuilder : IAuditModelBuilder
{
    private readonly Dictionary<Type, IEntityAuditBuilderInternal> _entityBuilders = new();

    public IAuditModelBuilder ApplyConfiguration<T>(IAuditEntityConfiguration<T> configuration)
    {
        var builder = new EntityAuditBuilder<T>();
        configuration.Configure(builder);
        _entityBuilders[typeof(T)] = builder;
        return this;
    }

    public IAuditModelBuilder ApplyConfigurationsFromAssembly(Assembly assembly)
    {
        var configTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IAuditEntityConfiguration<>))
                .Select(i => (ConfigType: t, EntityType: i.GetGenericArguments()[0])));

        foreach (var (configType, entityType) in configTypes)
        {
            var config = Activator.CreateInstance(configType)!;
            var method = typeof(AuditModelBuilder)
                .GetMethod(nameof(ApplyConfiguration), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
                .MakeGenericMethod(entityType);
            method.Invoke(this, new object[] { config });
        }

        return this;
    }

    internal AuditModel Build()
    {
        var configs = new Dictionary<Type, AuditEntityConfig>();
        foreach (var (type, builder) in _entityBuilders)
        {
            configs[type] = BuildEntityConfig(builder);
        }
        return new AuditModel(configs);
    }

    private static AuditEntityConfig BuildEntityConfig(IEntityAuditBuilderInternal builder)
    {
        return new AuditEntityConfig
        {
            OverrideName = builder.OverrideName,
            IsIgnored = builder.IsIgnored,
            Properties = builder.PropertyBuilders.ToDictionary(
                kvp => kvp.Key,
                kvp => new AuditPropertyConfig
                {
                    OverrideName = kvp.Value.OverrideName,
                    IsIgnored = kvp.Value.IsIgnored
                })
        };
    }
}
