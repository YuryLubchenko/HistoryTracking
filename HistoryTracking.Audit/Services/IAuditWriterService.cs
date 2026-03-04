using HistoryTracking.Audit.Entities;

namespace HistoryTracking.Audit.Services;

public interface IAuditWriterService
{
    Task<IAuditScope> CreateScopeAsync(AuditScopeDetails details);

    Task HandleEntityChangedAsync(object oldEntity, object newEntity, ActionType actionType);
}
