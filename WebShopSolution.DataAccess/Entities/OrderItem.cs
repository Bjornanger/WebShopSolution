using System.Text.Json.Serialization;

namespace WebShopSolution.DataAccess.Entities;

public class OrderItem
{
    public int OrderId { get; set; }
    [JsonIgnore]
    public Order? Order { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
}