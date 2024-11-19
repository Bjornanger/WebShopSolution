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
        
        
        //Kolla hur man ska lägga till generisk repo här

        public IRepositoryFactory _repositoryFactory { get; }
        private readonly Dictionary<Type, object> _repositories;

        private readonly ProductSubject _productSubject;



        // Konstruktor används för tillfället av Observer pattern
        //ProductSubject productSubject= null
        public UnitOfWork(MyDbContext context, IRepositoryFactory factory )
        {

            _context = context;
            _repositoryFactory = factory;
            _repositories = new Dictionary<Type, object>();


            // Om inget ProductSubject injiceras, skapa ett nytt
            //_productSubject = productSubject ?? new ProductSubject();

            // Registrera standardobservatörer
            //_productSubject.Attach(new EmailNotification());
        }

        //Denna metod hanterar vilken typ av Entity som kommer in och returnerar rätt repository
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories.TryGetValue(typeof(TEntity), out var existingRepository))
            {
                return (IRepository<TEntity>)existingRepository;
            }

            var repository = _repositoryFactory.GetSpecificRepository<TEntity>();
            _repositories[typeof(TEntity)] = repository;
            return repository;
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
