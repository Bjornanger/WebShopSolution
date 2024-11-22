namespace WebShopSolution.DataAccess.Repositories;

public interface IRepository <TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync(); // Hämtar alla 
    Task<bool> AddAsync(TEntity entity); // Lägger till
    Task<TEntity> UpdateAsync(TEntity entity); // Uppdaterar
    Task<bool> RemoveAsync(int id);
}