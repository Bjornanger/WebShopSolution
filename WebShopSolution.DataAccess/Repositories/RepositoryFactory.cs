using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Repositories;

public class RepositoryFactory() : IRepositoryFactory
{
    private readonly MyDbContext _context;

    public RepositoryFactory(MyDbContext context) : this()
    {
        this._context = context;
    }
    public IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class
    {
      return new Repository<TEntity>(_context);
    }
    
}