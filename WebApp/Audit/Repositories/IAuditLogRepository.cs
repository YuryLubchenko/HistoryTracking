using WebApp.Audit.Entities;

namespace WebApp.Audit.Repositories;

public interface IAuditLogRepository
{
    Task<long> SaveActionLog(ActionLogEntity actionLog);
    Task<long> SaveEntityChange(EntityRecordEntity entityRecord);
    Task SavePropertyChanges(IEnumerable<PropertyRecordEntity> propertyChanges);
}
