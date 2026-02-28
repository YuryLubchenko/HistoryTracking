using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("property_definitions", Schema = EntityDefaults.Schema)]
internal class PropertyDefinitionEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("entity_definition_id"), NotNull]
    public long EntityDefinitionId { get; set; }

    [Column("property_name"), NotNull]
    public string PropertyName { get; set; } = string.Empty;

    [Column("property_type"), NotNull]
    public string PropertyType { get; set; } = string.Empty;
}
