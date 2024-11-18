using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;

namespace WebShopSolution.DataAccess.Repositories.Products;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(MyDbContext context) : base(context)
    {
    }

    public bool UpdateProductStock(Product product, int quantity)
    {
        throw new NotImplementedException();
    }
}