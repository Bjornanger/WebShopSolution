using WebShopSolution.DataAccess.Entities;

namespace WebShopSolution.DataAccess.Repositories.Products
{
    // Gränssnitt för produktrepositoryt enligt Repository Pattern
    public interface IProductRepository : IRepository<Product>
    {
        bool UpdateProductStock(Product product, int quantity);
    }
}
