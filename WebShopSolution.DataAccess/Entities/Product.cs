using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Entities
{
    public class Product : IEntity
    {
        [Key]
        public int Id { get; set; } 
        [Required,MinLength(3)]
        public string Name { get; set; } 
        public double Price { get; set; }
        public int Stock { get; set; }
        [JsonIgnore]
        public ICollection<OrderItem> OrderProducts { get; set; } = new List<OrderItem>();
    }
}