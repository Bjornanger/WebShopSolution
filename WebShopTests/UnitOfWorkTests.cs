using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.DataAccess.Repositories.Customer;
using WebShopSolution.DataAccess.Repositories.Orders;
using WebShopSolution.DataAccess.Repositories.Products;
using WebShopSolution.DataAccess.UnitOfWork;

namespace WebShopTests
{
    public class UnitOfWorkTests
    {

        private readonly IRepositoryFactory _factory;
        
        private readonly IUnitOfWork _unitOfWork;

        private readonly IProductRepository _productRepository;

        
        public UnitOfWorkTests()
        {
         
        }

        



       
    }
}
