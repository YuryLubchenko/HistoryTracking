using HistoryTracking.Audit.Entities;

namespace HistoryTracking.Audit.Services;

public interface IAuditLogService
{
    Task HandleEntityChangedAsync(object oldEntity, object newEntity, ActionType actionType);
}
