using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;

namespace WebShopSolution.DataAccess.Repositories.Orders;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(MyDbContext context) : base(context)
    {
    }

    public bool ChangeOrderStatus(Order order)
    {
        throw new NotImplementedException();
    }
}