using LinqToDB;
using LinqToDB.Data;
using HistoryTracking.Audit.Entities;

namespace HistoryTracking.Audit.Repositories;

internal class AuditLogRepository : IAuditLogRepository
{
    private readonly DataConnection _db;

    public AuditLogRepository(DataConnection db)
    {
        _db = db;
    }

    public async Task<long> SaveActionLog(ActionLogEntity actionLog)
    {
        var id = await _db.InsertWithInt64IdentityAsync(actionLog);
        return id;
    }

    public async Task<long> SaveEntityChange(EntityRecordEntity entityRecord)
    {
        var id = await _db.InsertWithInt64IdentityAsync(entityRecord);
        return id;
    }

    public async Task SavePropertyChanges(IEnumerable<PropertyRecordEntity> propertyChanges)
    {
        foreach (var propertyChange in propertyChanges)
        {
            await _db.InsertAsync(propertyChange);
        }
    }
}
