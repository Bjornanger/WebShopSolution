namespace WebShopSolution.Shared.Interfaces
{
    // Gränssnitt för Unit of Work
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        // Sparar förändringar (om du använder en databas)
        Task CompleteAsync();
        
    }
}

