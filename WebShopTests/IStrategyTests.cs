using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using WebShop.Controllers;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Strategy;
using WebShopSolution.Shared.Interfaces;

namespace WebShopTests;

public class IStrategyTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Product> _productRepository;
    private readonly IDiscountStrategy _discountStrategy;
    private readonly DiscountStrategyFactory _discountStrategyFactory;
    private readonly DiscountContext _discountContext;
    private readonly ProductController _controller;

    public IStrategyTests()
    {

        // Initialisera fakes
        _unitOfWork = A.Fake<IUnitOfWork>();
        _productRepository = A.Fake<IRepository<Product>>();
        _discountStrategy = A.Fake<IDiscountStrategy>();
        _discountContext = A.Fake<DiscountContext>();
        _discountStrategyFactory = A.Fake<DiscountStrategyFactory>();

        // Initialisera controller
        _controller = new ProductController(_unitOfWork, _discountContext, _discountStrategyFactory);
    }

    //Måste fixa så att DateTime.Now returnerar en specifik dag för att testa BlackFriday


    #region TestCasesForProductControllerWithDiscountStrategy


    [Fact]
    public async Task AddProduct_WithBlackFridayDiscount_ReturnsOk()
    {
        // Arrange
        var product = new Product
        {
            Name = "Tvål",
            Price = 100,
            Stock = 23,
            OrderProducts = null
        };
        var currentDate = new DateTime(2024, 11, 29);//BlackFriday

        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(currentDate)).Returns(_discountStrategy); // Hämtar BlackFridayDiscount
        A.CallTo(() => _discountContext.SetDiscountStrategy(_discountStrategy)).DoesNothing(); // Sätter BlackFridayDiscount
        A.CallTo(() => _discountContext.GetDiscountPeriod(product)) .Returns(50); // Returnerar priset med rabatt
        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        
        // Act
        var result = await _controller.AddProduct(product);
        
        // Assert
        Assert.IsType<OkResult>(result); 
        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(currentDate)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _discountContext.SetDiscountStrategy(_discountStrategy)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _discountContext.GetDiscountPeriod(product)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _productRepository.AddAsync(product)).MustHaveHappenedOnceExactly(); 
        A.CallTo(() => _unitOfWork.CompleteAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetProductById_WithBlackFridayDiscount_ReturnsOk()
    {
        // Arrange

        // Act

        // Assert

    }

    [Fact]
    public async Task GetAllProducts_WithBlackFridayDiscount_ReturnsOk()
    {
        // Arrange

        // Act

        // Assert

    }


    #endregion

    #region DiscountStrategyFactoryTests
    [Fact]
    public async Task GetDiscountStrategy_BlackFridayMustHaveHappened_ReturnsBlackFridayDiscount()
    {
        // Arrange

        // Act

        // Assert

    }

    [Fact]
    public async Task GetDiscountStrategy_NoDiscountMustHaveHappened_ReturnsNoDiscount()
    {
        // Arrange
        // Act
        // Assert
    }


    [Fact]
    public async Task IsBlackFriday_MustHaveHappened_ReturnOk()
    {
        // Arrange
        // Act
        // Assert
    }
  

    #endregion

}