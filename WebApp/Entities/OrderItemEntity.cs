using LinqToDB.Mapping;

namespace WebApp.Entities;

[Table("order_items")]
public class OrderItemEntity : BaseEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public override long Id { get; set; }

    [Column("order_id"), NotNull]
    public long OrderId { get; set; }

    [Column("product_name"), NotNull]
    public string ProductName { get; set; } = string.Empty;

    [Column("quantity"), NotNull]
    public int Quantity { get; set; }

    [Column("unit_price"), NotNull]
    public decimal UnitPrice { get; set; }
}
