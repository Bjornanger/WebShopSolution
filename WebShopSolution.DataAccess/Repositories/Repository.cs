using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{

    //Consume db Context
    private readonly MyDbContext _context;

    private readonly DbSet<TEntity> _dbSet;


    public Repository(MyDbContext context)
    {
        this._context = context;
        this._dbSet = _context.Set<TEntity>();
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);

    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
        
    }

    public async Task<bool> AddAsync(TEntity entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        try
        {
            _dbSet.Update(entity);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveAsync(int id)
    {
        try
        {
            _dbSet.Remove(await GetByIdAsync(id));
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}