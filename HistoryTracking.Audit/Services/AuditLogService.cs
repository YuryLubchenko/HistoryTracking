using System.Reflection;
using LinqToDB.Mapping;
using HistoryTracking.Audit.Entities;
using HistoryTracking.Audit.Repositories;
using EntityActionType = HistoryTracking.Audit.Entities.ActionType;

namespace HistoryTracking.Audit.Services;

internal class AuditLogService : IAuditLogService
{
    private readonly IHistoryContext _historyContext;
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IHistoryContext historyContext, IAuditLogRepository auditLogRepository)
    {
        _historyContext = historyContext;
        _auditLogRepository = auditLogRepository;
    }

    public async Task HandleEntityChangedAsync(object oldEntity, object newEntity, ActionType actionType)
    {
        var actionLogId = await EnsureActionLogExists();

        var entity = oldEntity ?? newEntity;
        var entityName = GetEntityName(entity);
        var entityId = GetEntityId(entity);

        var entityTypeId = await _auditLogRepository.GetOrCreateEntityTypeIdAsync(entityName);

        var entityChange = new EntityRecordEntity
        {
            ActionLogId = actionLogId,
            EntityTypeId = entityTypeId,
            EntityId = entityId,
            ActionType = MapActionType(actionType)
        };

        var entityChangeId = await _auditLogRepository.SaveEntityChange(entityChange);

        var propertyChanges = PropertyComparer.Compare(oldEntity, newEntity);

        if (propertyChanges.Count > 0)
        {
            var propertyChangeEntities = new List<PropertyRecordEntity>();
            foreach (var pc in propertyChanges)
            {
                var propertyDefinitionId = await _auditLogRepository.GetOrCreatePropertyDefinitionIdAsync(entityTypeId, pc.PropertyName, pc.PropertyType);
                propertyChangeEntities.Add(new PropertyRecordEntity
                {
                    EntityRecordId = entityChangeId,
                    PropertyDefinitionId = propertyDefinitionId,
                    OldValue = pc.OldValue,
                    NewValue = pc.NewValue
                });
            }

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

    private static EntityActionType MapActionType(ActionType actionType)
    {
        return actionType switch
        {
            ActionType.Created => EntityActionType.Created,
            ActionType.Updated => EntityActionType.Updated,
            ActionType.Deleted => EntityActionType.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
        };
    }
}
