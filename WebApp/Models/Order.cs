namespace WebApp.Models;

public class Order
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}
