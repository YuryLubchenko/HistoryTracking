using LinqToDB;
using LinqToDB.Mapping;

namespace HistoryTracking.Audit.Entities;

[Table("property_records")]
internal class PropertyRecordEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("entity_record_id"), NotNull]
    public long EntityRecordId { get; set; }

    [Column("property_definition_id"), NotNull]
    public long PropertyDefinitionId { get; set; }

    [Column("old_value", DataType = DataType.Text)]
    public string OldValue { get; set; }

    [Column("new_value", DataType = DataType.Text)]
    public string NewValue { get; set; }
}
