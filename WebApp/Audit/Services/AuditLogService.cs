using System.Reflection;
using LinqToDB.Mapping;
using WebApp.Audit.Entities;
using WebApp.Audit.Events;
using WebApp.Audit.Repositories;
using EventActionType = WebApp.Audit.Events.ActionType;
using EntityActionType = WebApp.Audit.Entities.ActionType;

namespace WebApp.Audit.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IHistoryContext _historyContext;
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IHistoryContext historyContext, IAuditLogRepository auditLogRepository)
    {
        _historyContext = historyContext;
        _auditLogRepository = auditLogRepository;
    }

    public async Task HandleAsync(EntityChangedEvent entityChangedEvent)
    {
        var actionLogId = await EnsureActionLogExists();

        var entity = entityChangedEvent.OldEntity ?? entityChangedEvent.NewEntity;
        var entityName = GetEntityName(entity);
        var entityId = GetEntityId(entity);

        var entityChange = new EntityRecordEntity
        {
            ActionLogId = actionLogId,
            EntityName = entityName,
            EntityId = entityId,
            ActionType = MapActionType(entityChangedEvent.ActionType)
        };

        var entityChangeId = await _auditLogRepository.SaveEntityChange(entityChange);

        var propertyChanges = PropertyComparer.Compare(
            entityChangedEvent.OldEntity,
            entityChangedEvent.NewEntity);

        if (propertyChanges.Count > 0)
        {
            var propertyChangeEntities = propertyChanges.Select(pc => new PropertyRecordEntity
            {
                EntityRecordId = entityChangeId,
                PropertyName = pc.PropertyName,
                PropertyType = pc.PropertyType,
                OldValue = pc.OldValue,
                NewValue = pc.NewValue
            });

            await _auditLogRepository.SavePropertyChanges(propertyChangeEntities);
        }
    }

    private async Task<long> EnsureActionLogExists()
    {
        if (_historyContext.ActionLogId.HasValue)
        {
            return _historyContext.ActionLogId.Value;
        }

        var actionLog = new ActionLogEntity
        {
            Timestamp = DateTime.UtcNow
        };

        var actionLogId = await _auditLogRepository.SaveActionLog(actionLog);
        _historyContext.ActionLogId = actionLogId;

        return actionLogId;
    }

    private static string GetEntityName(object entity)
    {
        if (entity == null)
        {
            return string.Empty;
        }

        var typeName = entity.GetType().Name;
        return typeName.Replace("Entity", "");
    }

    private static long GetEntityId(object entity)
    {
        if (entity == null)
        {
            return 0;
        }

        var entityType = entity.GetType();
        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var primaryKeyAttribute = property.GetCustomAttribute<PrimaryKeyAttribute>();
            if (primaryKeyAttribute != null)
            {
                var value = property.GetValue(entity);
                return Convert.ToInt64(value);
            }
        }

        return 0;
    }

    private static EntityActionType MapActionType(EventActionType eventActionType)
    {
        return eventActionType switch
        {
            EventActionType.Created => EntityActionType.Created,
            EventActionType.Updated => EntityActionType.Updated,
            EventActionType.Deleted => EntityActionType.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(eventActionType), eventActionType, null)
        };
    }
}
