using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;

using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.DataAccess.Repositories.Customer;
using WebShopSolution.DataAccess.Repositories.Orders;
using WebShopSolution.DataAccess.Repositories.Products;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        // Hämta repository
        private readonly MyDbContext _context;
        public IRepositoryFactory _repositoryFactory { get; }
        private readonly Dictionary<Type, object> _repositories;
        

        public UnitOfWork(IRepositoryFactory factory)
        {
            _repositoryFactory = factory;
            _repositories = new Dictionary<Type, object>();
        }

        //Denna metod hanterar vilken typ av Entity som kommer in och returnerar rätt repository
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories.TryGetValue(typeof(TEntity), out var existingRepository))
            {
                return (IRepository<TEntity>)existingRepository;
            }

            var repository = _repositoryFactory.CreateRepository<TEntity>();
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
        
    }
}
