using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("hist_action_types")]
internal class ActionTypeEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("description"), NotNull]
    public string Description { get; set; }
}
