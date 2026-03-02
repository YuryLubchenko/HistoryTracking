using LinqToDB;
using LinqToDB.Data;
using HistoryTracking.Audit.Configuration;
using HistoryTracking.Audit.Entities;

namespace HistoryTracking.Audit.Repositories;

internal class AuditLogRepository : IAuditLogRepository
{
    private readonly DataConnection _db;
    private readonly string _schemaName;

    public AuditLogRepository(DataConnection db, AuditModel auditModel)
    {
        _db = db;
        _schemaName = auditModel.SchemaName;
    }

    public async Task<long> SaveActionLog(ActionLogEntity actionLog)
    {
        var id = await _db.InsertWithInt64IdentityAsync(actionLog, schemaName: _schemaName);
        return id;
    }

    public async Task<long> SaveEntityChange(EntityRecordEntity entityRecord)
    {
        var id = await _db.InsertWithInt64IdentityAsync(entityRecord, schemaName: _schemaName);
        return id;
    }

    public async Task SavePropertyChanges(IEnumerable<PropertyRecordEntity> propertyChanges)
    {
        foreach (var propertyChange in propertyChanges)
        {
            await _db.InsertAsync(propertyChange, schemaName: _schemaName);
        }
    }

    public async Task<long> GetOrCreateEntityTypeIdAsync(string name)
    {
        return await _db.ExecuteAsync<long>(
            $"INSERT INTO {_schemaName}.entity_definitions (name) VALUES (@name) ON CONFLICT (name) DO UPDATE SET name = EXCLUDED.name RETURNING id",
            new DataParameter("name", name));
    }

    public async Task<long> GetOrCreatePropertyDefinitionIdAsync(long entityTypeId, string propertyName, string propertyType)
    {
        return await _db.ExecuteAsync<long>(
            $"INSERT INTO {_schemaName}.property_definitions (entity_definition_id, property_name, property_type) VALUES (@entityTypeId, @propertyName, @propertyType) ON CONFLICT (entity_definition_id, property_name, property_type) DO UPDATE SET property_name = EXCLUDED.property_name RETURNING id",
            new DataParameter("entityTypeId", entityTypeId),
            new DataParameter("propertyName", propertyName),
            new DataParameter("propertyType", propertyType));
    }

    public async Task<long> GetOrCreateActionDefinitionIdAsync(string code, string name)
    {
        return await _db.ExecuteAsync<long>(
            $"INSERT INTO {_schemaName}.action_definitions (code, name) VALUES (@code, @name) ON CONFLICT (code) DO UPDATE SET name = EXCLUDED.name RETURNING id",
            new DataParameter("code", code),
            new DataParameter("name", name));
    }
}
