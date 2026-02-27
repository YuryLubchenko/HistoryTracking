using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("property_definitions")]
internal class PropertyDefinitionEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("entity_type_id"), NotNull]
    public long EntityTypeId { get; set; }

    [Column("property_name"), NotNull]
    public string PropertyName { get; set; } = string.Empty;

    [Column("property_type"), NotNull]
    public string PropertyType { get; set; } = string.Empty;
}
