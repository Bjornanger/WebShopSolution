using System.Net.Http.Headers;
using WebShopSolution.DataAccess.Entities;

namespace WebShopSolution.Shared.Interfaces;

public interface IDiscountStrategy
{
     double CalculatePrice(Product product);
}