﻿using WebShopSolution.DataAccess.Entities;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Repositories.Orders;

public interface IOrderRepository: IRepository<Order>
{
    bool ChangeOrderStatus(Order order);
}