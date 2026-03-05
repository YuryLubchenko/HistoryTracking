using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("hist_action_logs")]
internal class ActionLogEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("parent_action_log_id"), Nullable]
    public long? ParentActionLogId { get; set; }

    [Column("timestamp"), NotNull]
    public DateTime Timestamp { get; set; }

    [Column("client_id"), NotNull]
    public long ClientId { get; set; }

    [Column("action_type_id"), Nullable]
    public long? ActionTypeId { get; set; }
}
