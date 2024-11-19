namespace WebShopSolution.DataAccess.Repositories;

public interface IRepository <TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync(); // Hämtar alla 
    Task AddAsync(TEntity entity); // Lägger till
    Task UpdateAsync(TEntity entity); // Uppdaterar
    Task RemoveAsync(int id);
}