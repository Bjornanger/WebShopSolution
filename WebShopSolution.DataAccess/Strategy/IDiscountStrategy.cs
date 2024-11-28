using WebShopSolution.DataAccess.Entities;

namespace WebShopSolution.DataAccess.Strategy;

public interface IDiscountStrategy
{
     double CalculatePrice(Product product);
}