using System.Linq.Expressions;

namespace HistoryTracking.Audit.Configuration;

internal interface IEntityAuditBuilderInternal
{
    string OverrideName { get; }
    bool IsIgnored { get; }
    Dictionary<string, PropertyAuditBuilder> PropertyBuilders { get; }
}

internal class EntityAuditBuilder<T> : IEntityAuditBuilder<T>, IEntityAuditBuilderInternal
{
    public string OverrideName { get; private set; }
    public bool IsIgnored { get; private set; }
    public Dictionary<string, PropertyAuditBuilder> PropertyBuilders { get; } = new();

    public IEntityAuditBuilder<T> HasName(string name) { OverrideName = name; return this; }
    public IEntityAuditBuilder<T> Ignore()              { IsIgnored = true;    return this; }

    public IPropertyAuditBuilder Property(Expression<Func<T, object>> selector)
    {
        var name = ExtractPropertyName(selector);
        if (!PropertyBuilders.TryGetValue(name, out var pb))
            PropertyBuilders[name] = pb = new PropertyAuditBuilder();
        return pb;
    }

    private static string ExtractPropertyName(Expression<Func<T, object>> selector)
    {
        var body = selector.Body is UnaryExpression u ? u.Operand : selector.Body;
        return ((MemberExpression)body).Member.Name;
    }
}
