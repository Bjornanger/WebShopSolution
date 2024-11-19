using Microsoft.EntityFrameworkCore;

namespace WebShopSolution.DataAccess.Repositories;

public interface IRepositoryFactory
{

    //Detta är en generisk metod som returnerar ett repository
    public IRepository<TEntity> GetSpecificRepository<TEntity>() where TEntity : class;




}