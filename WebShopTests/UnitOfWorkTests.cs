using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.DataAccess.Repositories.Customer;
using WebShopSolution.DataAccess.Repositories.Orders;
using WebShopSolution.DataAccess.Repositories.Products;
using WebShopSolution.DataAccess.UnitOfWork;
using WebShopSolution.Shared.Interfaces;

namespace WebShopTests
{
    public class UnitOfWorkTests
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Order> _orderRepository;

        

        public UnitOfWorkTests()
        {
            _productRepository = A.Fake<IRepository<Product>>();
            _customerRepository = A.Fake<IRepository<Customer>>();
            _orderRepository = A.Fake<IRepository<Order>>();

            _unitOfWork = A.Fake<IUnitOfWork>();
        }
        

        [Fact]
        public void RepositoryMethod_ThruUnitOfWork_ReturnsProductRepository()
        {
            // Arrange
            A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
            
            // Act
           var result = _unitOfWork.Repository<Product>();

            // Assert
            Assert.Equal(_productRepository, result);
            Assert.IsAssignableFrom<IRepository<Product>>(result);
            A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();
        }
        [Fact]
        public  void RepositoryMethod_ThruUnitOfWork_ReturnsCustomerRepository()
        {
            // Arrange
            A.CallTo(() => _unitOfWork.Repository<Customer>()).Returns(_customerRepository);

            // Act
            var result = _unitOfWork.Repository<Customer>();

            // Assert
            Assert.Equal(_customerRepository, result);
            Assert.IsAssignableFrom<IRepository<Customer>>(result);
            A.CallTo(() => _unitOfWork.Repository<Customer>()).MustHaveHappened();
        }
        [Fact]
        public  void RepositoryMethod_ThruUnitOfWork_ReturnsOrderRepository()
        {
            // Arrange
            A.CallTo(() => _unitOfWork.Repository<Order>()).Returns(_orderRepository);

            // Act
            var result = _unitOfWork.Repository<Order>();

            // Assert
            Assert.Equal(_orderRepository, result);
            Assert.IsAssignableFrom<IRepository<Order>>(result);
            A.CallTo(() => _unitOfWork.Repository<Order>()).MustHaveHappened();
        }

        [Fact]
        public async Task CompleteAsync_CallsTheMethod()
        {
            // Arrange
            A.CallTo(() => _unitOfWork.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            await _unitOfWork.CompleteAsync();

            // Assert
            A.CallTo(() => _unitOfWork.CompleteAsync()).MustHaveHappened();
        }
        
        [Fact]
        public void Dispose_CallsTheMethodDispose()
        {
            // Arrange
            A.CallTo(() => _unitOfWork.Dispose());
            // Act
            _unitOfWork.Dispose();
            // Assert
            A.CallTo(() => _unitOfWork.Dispose()).MustHaveHappened();
        }
    }
}
