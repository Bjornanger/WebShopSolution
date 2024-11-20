﻿using Microsoft.EntityFrameworkCore;
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

    public async Task AddAsync(TEntity entity)
    {
       await _dbSet.AddAsync(entity);
    }

    public async Task UpdateAsync(TEntity entity)
    {
       _dbSet.Update(entity);
       
    }

    public async Task RemoveAsync(int id)
    {
      _dbSet.Remove(await GetByIdAsync(id));
      
    }
}