using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Controllers;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.DataAccess.Strategy;
using WebShopSolution.DataAccess.Strategy.DateTimeHelper;
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

        private readonly DiscountContext _discountContext;
        private readonly DiscountStrategyFactory _discountStrategyFactory;
        private readonly IDateTimeProvider _dateTimeProvider;



        private readonly ProductController _controller;


        public UnitOfWorkTests()
        {
            _productRepository = A.Fake<IRepository<Product>>();
            _customerRepository = A.Fake<IRepository<Customer>>();
            _orderRepository = A.Fake<IRepository<Order>>();
            _unitOfWork = A.Fake<IUnitOfWork>();

            _controller = new ProductController(_unitOfWork, _discountContext, _discountStrategyFactory,
                _dateTimeProvider);
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
        public void RepositoryMethod_ThruUnitOfWork_ReturnsCustomerRepository()
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
        public void RepositoryMethod_ThruUnitOfWork_ReturnsOrderRepository()
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
            // Act
            await _unitOfWork.CompleteAsync();

            // Assert
            A.CallTo(() => _unitOfWork.CompleteAsync()).MustHaveHappened();
        }

        [Fact]
        public async void Dispose_CallsTheMethod()
        {
            //Arrange
            var product = A.Dummy<Product>();

            A.CallTo(() => _productRepository.GetByIdAsync(A<int>._)).Returns(Task.FromResult(product));
            A.CallTo(() => _productRepository.AddAsync(product)).Throws(new Exception("Test"));
            A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);

            //Act
            var result = await _controller.AddProduct(product);

            //Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error", objectResult.Value);
            A.CallTo(() => _unitOfWork.Dispose()).MustHaveHappened();
        }
    }
}
