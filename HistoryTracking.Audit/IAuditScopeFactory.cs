namespace HistoryTracking.Audit;

internal interface IAuditScopeFactory
{
    Task<AuditScope> CreateScopeAsync(long clientId, AuditScopeDetails details);
    Task<AuditScope> GetOrCreateActionScopeAsync(long clientId);
}
