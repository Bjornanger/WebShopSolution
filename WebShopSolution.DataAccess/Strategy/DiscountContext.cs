using WebShopSolution.DataAccess.Entities;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Strategy;

public class DiscountContext
{
    private IDiscountStrategy _discountStrategy;
    
    public virtual void SetDiscountStrategy(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }
    
    public virtual double GetDiscountPeriod(Product product)
    {
        return _discountStrategy.CalculatePrice(product);
    }

}