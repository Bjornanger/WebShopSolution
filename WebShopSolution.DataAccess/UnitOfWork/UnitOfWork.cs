using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Notifications;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.DataAccess.Repositories.Customer;
using WebShopSolution.DataAccess.Repositories.Orders;
using WebShopSolution.DataAccess.Repositories.Products;

namespace WebShopSolution.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        // Hämta repository
        private readonly MyDbContext _context;


        public IProductRepository Products { get; }
        public ICustomerRepository Customers { get; }
        public IOrderRepository Orders { get; }
        



        private readonly ProductSubject _productSubject;



        // Konstruktor används för tillfället av Observer pattern
        public UnitOfWork(MyDbContext context, ProductSubject productSubject= null)
        {

            _context = context;

            Products = new ProductRepository(_context);
            Orders = new OrderRepository(_context);
            Customers = new CustomerRepository(_context);

            

            // Om inget ProductSubject injiceras, skapa ett nytt
            _productSubject = productSubject ?? new ProductSubject();

            // Registrera standardobservatörer
            _productSubject.Attach(new EmailNotification());
        }

      

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
          _context.Dispose();
        }
        

        public void NotifyProductAdded(Product product)
        {
            _productSubject.Notify(product);
        }

      
    }
}
