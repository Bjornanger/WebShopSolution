namespace WebShopSolution.Shared.Interfaces
{
    
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Task CompleteAsync();
        
    }
}

