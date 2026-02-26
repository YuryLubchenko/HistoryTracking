using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("entity_records")]
internal class EntityRecordEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("action_log_id"), NotNull]
    public long ActionLogId { get; set; }

    [Column("entity_name"), NotNull]
    public string EntityName { get; set; } = string.Empty;

    [Column("entity_id"), NotNull]
    public long EntityId { get; set; }

    [Column("action_type"), NotNull]
    public ActionType ActionType { get; set; }
}
