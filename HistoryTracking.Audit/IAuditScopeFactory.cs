namespace HistoryTracking.Audit;

internal interface IAuditScopeFactory
{
    Task<IAuditScope> CreateScopeAsync(AuditScopeDetails details);
    Task<long> GetOrCreateActionLogIdAsync();
}
