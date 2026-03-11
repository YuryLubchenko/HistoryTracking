namespace HistoryTracking.Audit.Services;

public interface IAuditQueryService
{
    Task<long> GetActionLogCountAsync(long clientId);
}
