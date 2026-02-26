using LinqToDB.Mapping;

namespace WebApp.Entities;

[Table("orders")]
public class OrderEntity : BaseEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public override long Id { get; set; }

    [Column("client_id"), NotNull]
    public long ClientId { get; set; }

    [Column("order_date"), NotNull]
    public DateTime OrderDate { get; set; }

    [Column("total_amount"), NotNull]
    public decimal TotalAmount { get; set; }
}
