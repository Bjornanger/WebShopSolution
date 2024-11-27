namespace WebShopSolution.Shared.Interfaces;

public interface IRepositoryFactory
{
    public IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;
}