using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Entities;

public class Order : IEntity
{
    [Key]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    
    public ICollection<OrderItem> OrderProducts { get; set; } = new List<OrderItem>();

    public int Quantity { get; set; }

}