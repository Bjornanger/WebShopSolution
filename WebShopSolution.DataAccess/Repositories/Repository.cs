using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{

    private readonly MyDbContext _context;

    private readonly DbSet<TEntity> _dbSet;


    public Repository(MyDbContext context)
    {
        this._context = context;
        this._dbSet = _context.Set<TEntity>();
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        try
        {
            return await _dbSet.FindAsync(id);
        }
        catch
        {
            return null;
        }

    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await _dbSet.ToListAsync();
        }
        catch
        {
            return new List<TEntity>();
        } 
        
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

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        try
        {
           _dbSet.Update(entity);
            return entity;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> RemoveAsync(int id)
    {
        try
        {
            _dbSet.Remove(await GetByIdAsync(id));
            return true;
        }
        catch 
        {
            return false;
        }
    }
}