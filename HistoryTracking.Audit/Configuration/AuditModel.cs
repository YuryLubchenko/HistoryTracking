namespace HistoryTracking.Audit.Configuration;

internal class AuditModel
{
    private readonly Dictionary<Type, AuditEntityConfig> _configs;

    internal string SchemaName { get; }

    internal AuditModel(Dictionary<Type, AuditEntityConfig> configs, string schemaName)
    {
        _configs = configs;
        SchemaName = schemaName;
    }

    internal AuditEntityConfig GetEntityConfig(Type type) =>
        _configs.TryGetValue(type, out var cfg) ? cfg : null;
}

internal class AuditEntityConfig
{
    internal string OverrideName { get; init; }
    internal bool IsIgnored { get; init; }
    internal IReadOnlyDictionary<string, AuditPropertyConfig> Properties { get; init; }
        = new Dictionary<string, AuditPropertyConfig>();

    internal AuditPropertyConfig GetPropertyConfig(string name) =>
        Properties.TryGetValue(name, out var cfg) ? cfg : null;
}

internal class AuditPropertyConfig
{
    internal string OverrideName { get; init; }
    internal bool IsIgnored { get; init; }
}
