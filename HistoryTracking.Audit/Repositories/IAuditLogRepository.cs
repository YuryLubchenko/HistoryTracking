using HistoryTracking.Audit.Entities;

namespace HistoryTracking.Audit.Repositories;

internal interface IAuditLogRepository
{
    Task<long> SaveActionLog(ActionLogEntity actionLog);
    Task<long> SaveEntityChange(EntityRecordEntity entityRecord);
    Task SavePropertyChanges(IEnumerable<PropertyRecordEntity> propertyChanges);
    Task<long> GetOrCreateEntityTypeIdAsync(string name);
    Task<long> GetOrCreatePropertyDefinitionIdAsync(long entityTypeId, string propertyName, string propertyType);
}
