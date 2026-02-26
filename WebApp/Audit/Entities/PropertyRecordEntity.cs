using LinqToDB;
using LinqToDB.Mapping;

namespace WebApp.Audit.Entities;

[Table("property_records")]
public class PropertyRecordEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public long Id { get; set; }

    [Column("entity_record_id"), NotNull]
    public long EntityRecordId { get; set; }

    [Column("property_name"), NotNull]
    public string PropertyName { get; set; } = string.Empty;

    [Column("property_type"), NotNull]
    public string PropertyType { get; set; } = string.Empty;

    [Column("old_value", DataType = DataType.BinaryJson)]
    public string OldValue { get; set; }

    [Column("new_value", DataType = DataType.BinaryJson)]
    public string NewValue { get; set; }
}
