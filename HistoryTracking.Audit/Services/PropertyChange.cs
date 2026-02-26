namespace HistoryTracking.Audit.Services;

internal class PropertyChange
{
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string OldValue { get; set; }
    public string NewValue { get; set; }
}
