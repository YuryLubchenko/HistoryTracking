using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("entity_types")]
internal class EntityTypeEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("name"), NotNull]
    public string Name { get; set; } = string.Empty;
}
