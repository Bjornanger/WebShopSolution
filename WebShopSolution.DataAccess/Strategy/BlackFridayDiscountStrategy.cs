using WebShopSolution.DataAccess.Entities;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Strategy;

public class BlackFridayDiscountStrategy : IDiscountStrategy
{

    private readonly double _discountRate;

    public BlackFridayDiscountStrategy(double discountRate)
    {
        _discountRate = discountRate;
    }


    public double CalculatePrice(Product product)
    {
       return product.Price - (product.Price * _discountRate / 100);
    }
}