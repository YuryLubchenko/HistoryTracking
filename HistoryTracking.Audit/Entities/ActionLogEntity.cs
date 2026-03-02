using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("action_logs", Schema = EntityDefaults.Schema)]
internal class ActionLogEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("timestamp"), NotNull]
    public DateTime Timestamp { get; set; }

    [Column("action_definition_id"), Nullable]
    public long? ActionDefinitionId { get; set; }

    [Column("parent_action_log_id"), Nullable]
    public long? ParentActionLogId { get; set; }
}
