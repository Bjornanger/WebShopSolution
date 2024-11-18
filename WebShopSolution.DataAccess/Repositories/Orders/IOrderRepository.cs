using WebShopSolution.DataAccess.Entities;

namespace WebShopSolution.DataAccess.Repositories.Orders;

public interface IOrderRepository: IRepository<Order>
{
    bool ChangeOrderStatus(Order order);
}