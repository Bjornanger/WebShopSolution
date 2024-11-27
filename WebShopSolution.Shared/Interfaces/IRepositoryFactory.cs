namespace WebShopSolution.Shared.Interfaces;

public interface IRepositoryFactory
{

    //Detta är en generisk metod som returnerar ett repository
    public IRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;




}