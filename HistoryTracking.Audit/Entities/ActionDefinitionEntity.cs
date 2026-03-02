using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("action_definitions", Schema = EntityDefaults.Schema)]
internal class ActionDefinitionEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("code"), NotNull]
    public string Code { get; set; }

    [Column("name"), NotNull]
    public string Name { get; set; }
}
