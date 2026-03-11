using LinqToDB;
using LinqToDB.Data;
using HistoryTracking.Audit.Configuration;
using HistoryTracking.Audit.Entities;

namespace HistoryTracking.Audit.Services;

internal class AuditQueryService : IAuditQueryService
{
    private readonly DataConnection _db;
    private readonly string _schemaName;

    public AuditQueryService(DataConnection db, AuditModel auditModel)
    {
        _db = db;
        _schemaName = auditModel.SchemaName;
    }

    public Task<long> GetActionLogCountAsync(long clientId)
        => _db.GetTable<ActionLogEntity>()
              .SchemaName(_schemaName)
              .Where(x => x.ClientId == clientId)
              .LongCountAsync();
}
