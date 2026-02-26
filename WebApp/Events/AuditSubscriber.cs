using HistoryTracking.Audit.Services;
using AuditActionType = HistoryTracking.Audit.Services.ActionType;

namespace WebApp.Events;

public class AuditSubscriber : IEntityChangedHandler
{
    private readonly IAuditLogService _auditLogService;

    public AuditSubscriber(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    public async Task HandleAsync(EntityChangedEvent entityChangedEvent)
    {
        var actionType = MapActionType(entityChangedEvent.ActionType);
        await _auditLogService.HandleEntityChangedAsync(
            entityChangedEvent.OldEntity,
            entityChangedEvent.NewEntity,
            actionType);
    }

    private static AuditActionType MapActionType(ActionType actionType)
    {
        return actionType switch
        {
            ActionType.Created => AuditActionType.Created,
            ActionType.Updated => AuditActionType.Updated,
            ActionType.Deleted => AuditActionType.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
        };
    }
}
