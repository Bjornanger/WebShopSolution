namespace WebShopSolution.DataAccess.Repositories;

public interface IRepository <T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync(); // Hämtar alla 
    Task AddAsync(T entity); // Lägger till
    Task UpdateAsync(T entity); // Uppdaterar
    Task RemoveAsync(int id);
}