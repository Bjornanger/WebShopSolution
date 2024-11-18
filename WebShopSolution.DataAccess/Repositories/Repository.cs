using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Repositories;

public class Repository<T> : IRepository<T> where T : class
{

    //Consume db Context
    private readonly MyDbContext _context;

    private readonly DbSet<T> _dbSet;


    public Repository(MyDbContext context)
    {
        this._context = context;
        this._dbSet = _context.Set<T>();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);

    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
        
    }

    public async Task AddAsync(T entity)
    {
       await _dbSet.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
       _dbSet.Update(entity);

    }

    public async Task RemoveAsync(int id)
    {
      _dbSet.Remove(await GetByIdAsync(id));

        
    }
}