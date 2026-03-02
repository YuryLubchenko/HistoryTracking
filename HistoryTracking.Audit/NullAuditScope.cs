namespace HistoryTracking.Audit;

internal sealed class NullAuditScope : IAuditScope
{
    internal static readonly NullAuditScope Instance = new();
    public void Dispose() { }
}
