namespace HistoryTracking.Audit.Configuration;

internal class PropertyAuditBuilder : IPropertyAuditBuilder
{
    internal string OverrideName { get; private set; }
    internal bool IsIgnored { get; private set; }

    public IPropertyAuditBuilder HasName(string name) { OverrideName = name; return this; }
    public IPropertyAuditBuilder Ignore()              { IsIgnored = true;    return this; }
}
