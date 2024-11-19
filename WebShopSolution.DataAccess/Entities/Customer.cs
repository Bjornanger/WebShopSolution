using System.Text.Json.Serialization;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Entities;

public class Customer: IEntity
{
    public int  Id { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }
    [JsonIgnore]
    public ICollection<Order> Orders { get; set; } = new List<Order>();

}