namespace HistoryTracking.Audit;

public interface IAuditScopeFactory
{
    Task<IAuditScope> CreateScopeAsync(AuditScopeDetails details);
    internal Task<long> GetOrCreateActionLogIdAsync();
}
