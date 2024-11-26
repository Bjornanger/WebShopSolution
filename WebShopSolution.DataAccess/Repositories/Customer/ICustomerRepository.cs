namespace WebShopSolution.DataAccess.Repositories.Customer;

public interface ICustomerRepository : IRepository<Entities.Customer>
{
    bool ChangePassword(string password);

}