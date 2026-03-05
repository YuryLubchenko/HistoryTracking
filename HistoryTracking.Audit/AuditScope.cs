namespace HistoryTracking.Audit;

internal sealed class AuditScope : IAuditScope
{
    private readonly Action _onDispose;
    private bool _disposed;

    public long ActionLogId { get; }

    public AuditScope(long actionLogId, Action onDispose)
    {
        ActionLogId = actionLogId;
        _onDispose  = onDispose;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _onDispose();
        }
    }
}
