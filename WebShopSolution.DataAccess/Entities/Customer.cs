using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Entities;

public class Customer: IEntity
{
    public int  Id { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();

}