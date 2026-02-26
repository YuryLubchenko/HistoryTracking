using LinqToDB.Mapping;

namespace WebApp.Audit.Entities;

[Table("action_logs")]
public class ActionLogEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("timestamp"), NotNull]
    public DateTime Timestamp { get; set; }
}
