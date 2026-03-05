using HistoryTracking.Audit.Services;
using WebApp.Entities;
using AuditActionType = HistoryTracking.Audit.Services.ActionType;

namespace WebApp.Events;

public class AuditSubscriber : IEntityChangedHandler<BaseEntity>
{
    private readonly IAuditWriterService _auditWriterService;

    public AuditSubscriber(IAuditWriterService auditWriterService)
    {
        _auditWriterService = auditWriterService;
    }

    public async Task HandleAsync(EntityChangedEvent<BaseEntity> entityChangedEvent)
    {
        var newEntity = entityChangedEvent.NewEntity;
        var oldEntity = entityChangedEvent.OldEntity;

        var entity = newEntity ?? oldEntity;

        var clientId = GetClientId(entity);

        var actionType = MapActionType(entityChangedEvent.ActionType);
        await _auditWriterService.HandleEntityChangedAsync(
            clientId,
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

    private static long GetClientId<T>(T entity)
    {
        return entity switch
        {
            ClientEntity c => c.Id,
            ContactEntity c => c.ClientId,
            _ => throw new ArgumentException($"Unknown type {entity.GetType().Namespace}")
        };
    }
}
