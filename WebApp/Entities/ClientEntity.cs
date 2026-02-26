using LinqToDB.Mapping;

namespace WebApp.Entities;

[Table("clients")]
public class ClientEntity : BaseEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public override long Id { get; set; }

    [Column("name"), NotNull]
    public string Name { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; }

    [Column("phone")]
    public string Phone { get; set; }
}
