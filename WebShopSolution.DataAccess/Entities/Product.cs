using System.Text.Json.Serialization;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Entities
{
    // Produktmodellen representerar en produkt i webbshoppen
    public class Product : IEntity
    {
        public int Id { get; set; } // Unikt ID f�r produkten
        public string Name { get; set; } // Namn p� produkten
        public double Price { get; set; }
        public int Stock { get; set; }
        [JsonIgnore]
        public ICollection<OrderItem> OrderProducts { get; set; } = new List<OrderItem>();
    }
}