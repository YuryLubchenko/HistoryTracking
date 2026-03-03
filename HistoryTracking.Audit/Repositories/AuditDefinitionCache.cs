using System.Collections.Concurrent;

namespace HistoryTracking.Audit.Repositories;

internal class AuditDefinitionCache
{
    public ConcurrentDictionary<string, long> EntityDefinitions { get; } = new();
    public ConcurrentDictionary<string, long> ActionDefinitions { get; } = new();
    public ConcurrentDictionary<string, long> PropertyDefinitions { get; } = new();

    public static string PropertyKey(long entityTypeId, string propertyName, string propertyType)
        => $"{entityTypeId}:{propertyName}:{propertyType}";
}
