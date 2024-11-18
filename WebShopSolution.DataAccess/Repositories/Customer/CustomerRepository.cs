using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;

namespace WebShopSolution.DataAccess.Repositories.Customer;

public class CustomerRepository : Repository<Entities.Customer>, ICustomerRepository
{
    public CustomerRepository(MyDbContext context) : base(context)
    {
    }
}