using System.Reflection;
using LinqToDB.Mapping;
using HistoryTracking.Audit.Configuration;
using HistoryTracking.Audit.Entities;
using HistoryTracking.Audit.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using EntityActionType = HistoryTracking.Audit.Entities.ActionType;

namespace HistoryTracking.Audit.Services;

internal class AuditWriterService : IAuditWriterService
{
    private readonly IAuditScopeFactory _auditScopeFactory;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly AuditModel _auditModel;
    private readonly IOptions<AuditOptions> _auditOptions;
    private readonly IFeatureManager _featureManager;

    public AuditWriterService(
        IAuditScopeFactory auditScopeFactory,
        IAuditLogRepository auditLogRepository,
        AuditModel auditModel,
        IOptions<AuditOptions> auditOptions,
        IFeatureManager featureManager)
    {
        _auditScopeFactory = auditScopeFactory;
        _auditLogRepository = auditLogRepository;
        _auditModel = auditModel;
        _auditOptions = auditOptions;
        _featureManager = featureManager;
    }

    public async Task<IAuditScope> CreateScopeAsync(AuditScopeDetails details)
    {
        var toggleName = _auditOptions.Value.FeatureToggleName;
        if (!string.IsNullOrEmpty(toggleName) && !await _featureManager.IsEnabledAsync(toggleName))
            return NullAuditScope.Instance;

        return await _auditScopeFactory.CreateScopeAsync(details);
    }

    public async Task HandleEntityChangedAsync(object oldEntity, object newEntity, ActionType actionType)
    {
        var toggleName = _auditOptions.Value.FeatureToggleName;
        if (!string.IsNullOrEmpty(toggleName) && !await _featureManager.IsEnabledAsync(toggleName))
            return;

        var entity = oldEntity ?? newEntity;
        var entityConfig = _auditModel.GetEntityConfig(entity.GetType());

        if (entityConfig?.IsIgnored == true)
            return;

        var actionLogId = await _auditScopeFactory.GetOrCreateActionLogIdAsync();

        var entityName = entityConfig?.OverrideName ?? GetEntityName(entity);
        var entityId = GetEntityId(entity);

        var entityTypeId = await _auditLogRepository.GetOrCreateEntityTypeIdAsync(entityName);

        var entityChange = new EntityRecordEntity
        {
            ActionLogId = actionLogId,
            EntityDefinitionId = entityTypeId,
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
                var propConfig = entityConfig?.GetPropertyConfig(pc.PropertyName);
                if (propConfig?.IsIgnored == true)
                    continue;

                var propertyName = propConfig?.OverrideName ?? pc.PropertyName;
                var propertyDefinitionId = await _auditLogRepository.GetOrCreatePropertyDefinitionIdAsync(entityTypeId, propertyName, pc.PropertyType);
                propertyChangeEntities.Add(new PropertyRecordEntity
                {
                    EntityRecordId = entityChangeId,
                    PropertyDefinitionId = propertyDefinitionId,
                    OldValue = pc.OldValue,
                    NewValue = pc.NewValue
                });
            }

            if (propertyChangeEntities.Count > 0)
                await _auditLogRepository.SavePropertyChanges(propertyChangeEntities);
        }
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
