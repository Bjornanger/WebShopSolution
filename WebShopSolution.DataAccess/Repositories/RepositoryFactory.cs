﻿using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories.Customer;
using WebShopSolution.DataAccess.Repositories.Orders;
using WebShopSolution.DataAccess.Repositories.Products;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Repositories;

public class RepositoryFactory() : IRepositoryFactory
{
    private readonly MyDbContext _context;

    public RepositoryFactory(MyDbContext context) : this()
    {

        this._context = context;
    }

    //Lägg in en Switch-sats här senare för de andra entity
    public IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class
    {
        if (typeof(TEntity) == typeof(Product))
            return (IRepository<TEntity>)
                new ProductRepository(_context);

        if (typeof(TEntity) == typeof(Entities.Customer))
            return (IRepository<TEntity>)
                new CustomerRepository(_context);

        if (typeof(TEntity) == typeof(Order))
            return (IRepository<TEntity>)
                new OrderRepository(_context);






        return new Repository<TEntity>(_context);

    }


}