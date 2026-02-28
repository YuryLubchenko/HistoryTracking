using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("entity_definitions", Schema = EntityDefaults.Schema)]
internal class EntityDefinitionEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("name"), NotNull]
    public string Name { get; set; } = string.Empty;
}
