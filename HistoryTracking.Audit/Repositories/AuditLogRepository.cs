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
        var existing = await _db.GetTable<EntityDefinitionEntity>()
            .SchemaName(_schemaName)
            .Where(x => x.Name == name)
            .Select(x => (long?)x.Id)
            .FirstOrDefaultAsync();

        if (existing.HasValue)
            return existing.Value;

        var id = await _db.ExecuteAsync<long>(
            $@"INSERT INTO {GetSchemaPrefix()}hist_entity_definitions (name) VALUES (@name)
            ON CONFLICT (name) DO UPDATE SET name = EXCLUDED.name
            RETURNING id",
            new DataParameter("name", name));

        return id;
    }

    public async Task<long> GetOrCreatePropertyDefinitionIdAsync(long entityTypeId, string propertyName, string propertyType)
    {
        var existing = await _db.GetTable<PropertyDefinitionEntity>()
            .SchemaName(_schemaName)
            .Where(x => x.EntityDefinitionId == entityTypeId && x.PropertyName == propertyName && x.PropertyType == propertyType)
            .Select(x => (long?)x.Id)
            .FirstOrDefaultAsync();

        if (existing.HasValue)
            return existing.Value;

        var id = await _db.ExecuteAsync<long>(
            $@"INSERT INTO {GetSchemaPrefix()}hist_property_definitions (entity_definition_id, property_name, property_type) VALUES (@entityTypeId, @propertyName, @propertyType)
            ON CONFLICT (entity_definition_id, property_name, property_type) DO UPDATE SET property_name = EXCLUDED.property_name
            RETURNING id",
            new DataParameter("entityTypeId", entityTypeId),
            new DataParameter("propertyName", propertyName),
            new DataParameter("propertyType", propertyType));

        return id;
    }

    private string GetSchemaPrefix()
    {
        if (string.IsNullOrEmpty(_schemaName))
        {
            return string.Empty;
        }

        return $"{_schemaName}.";
    }
}
