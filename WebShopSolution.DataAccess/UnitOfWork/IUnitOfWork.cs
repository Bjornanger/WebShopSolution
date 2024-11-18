using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.DataAccess.Repositories.Customer;
using WebShopSolution.DataAccess.Repositories.Orders;
using WebShopSolution.DataAccess.Repositories.Products;

namespace WebShopSolution.DataAccess.UnitOfWork
{
    // Gränssnitt för Unit of Work
    public interface IUnitOfWork : IDisposable
    {
        // Repository för produkter, Customers, Orders

        IProductRepository Products { get; }
        ICustomerRepository Customers { get; }
        IOrderRepository Orders { get; }

        Task CompleteAsync();


        // Sparar förändringar (om du använder en databas)

        

        void NotifyProductAdded(Product product); // Notifierar observatörer om ny produkt
    }
}

