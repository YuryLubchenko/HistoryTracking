using HistoryTracking.Audit.Entities;

namespace HistoryTracking.Audit.Services;

public interface IAuditWriterService
{
    Task<IAuditScope> CreateScopeAsync(long clientId, AuditScopeDetails details);

    Task HandleEntityChangedAsync<T>(long clientId, T oldEntity, T newEntity, ActionType actionType);
}
