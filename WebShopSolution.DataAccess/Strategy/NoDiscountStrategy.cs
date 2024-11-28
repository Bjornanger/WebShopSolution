using WebShopSolution.DataAccess.Entities;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Strategy;

public class NoDiscountStrategy : IDiscountStrategy
{
    public virtual double CalculatePrice(Product product)
    {
        return product.Price;
    }
}