using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("action_logs")]
internal class ActionLogEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("timestamp"), NotNull]
    public DateTime Timestamp { get; set; }
}
