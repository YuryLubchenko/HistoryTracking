using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("hist_entity_definitions")]
internal class EntityDefinitionEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("name"), NotNull]
    public string Name { get; set; } = string.Empty;
}
