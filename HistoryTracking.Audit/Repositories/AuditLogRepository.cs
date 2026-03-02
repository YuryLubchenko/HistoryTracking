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

    public async Task<long> GetOrCreateEntityTypeIdAsync(string name)
    {
        return await _db.ExecuteAsync<long>(
            $"INSERT INTO {EntityDefaults.Schema}.entity_definitions (name) VALUES (@name) ON CONFLICT (name) DO UPDATE SET name = EXCLUDED.name RETURNING id",
            new DataParameter("name", name));
    }

    public async Task<long> GetOrCreatePropertyDefinitionIdAsync(long entityTypeId, string propertyName, string propertyType)
    {
        return await _db.ExecuteAsync<long>(
            $"INSERT INTO {EntityDefaults.Schema}.property_definitions (entity_definition_id, property_name, property_type) VALUES (@entityTypeId, @propertyName, @propertyType) ON CONFLICT (entity_definition_id, property_name, property_type) DO UPDATE SET property_name = EXCLUDED.property_name RETURNING id",
            new DataParameter("entityTypeId", entityTypeId),
            new DataParameter("propertyName", propertyName),
            new DataParameter("propertyType", propertyType));
    }

    public async Task<long> GetOrCreateActionDefinitionIdAsync(string code, string name)
    {
        return await _db.ExecuteAsync<long>(
            $"INSERT INTO {EntityDefaults.Schema}.action_definitions (code, name) VALUES (@code, @name) ON CONFLICT (code) DO UPDATE SET name = EXCLUDED.name RETURNING id",
            new DataParameter("code", code),
            new DataParameter("name", name));
    }
}
