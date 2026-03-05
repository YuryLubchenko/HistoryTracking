using LinqToDB;
using LinqToDB.Mapping;

namespace WebApp.Entities;

[Table("contacts")]
public class ContactEntity : BaseEntity
{
    [PrimaryKey]
    [Identity]
    [Column("id")]
    public override long Id { get; set; }

    [Column("client_id")]
    public long ClientId { get; set; }

    [Column("contact_type")]
    [DataType(DataType.Int64)]
    public ContactType ContactType { get; set; }

    [Column("value")]
    [NotNull]
    public string Value { get; set; }
}