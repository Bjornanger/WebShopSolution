using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection.Metadata;
using WebShop.Controllers;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Strategy;
using WebShopSolution.DataAccess.Strategy.DateTimeHelper;
using WebShopSolution.Shared.Interfaces;
using static WebShopTests.IStrategyTests;

namespace WebShopTests;

public class IStrategyTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Order> _orderRepository;

    private readonly IDiscountStrategy _discountStrategy;
    private readonly DiscountStrategyFactory _discountStrategyFactory;
    private readonly DiscountContext _discountContext;
    private readonly BlackFridayDiscountStrategy _blackFridayDiscountStrategy;
    private readonly NoDiscountStrategy _noDiscountStrategy;
    private readonly IDateTimeProvider _dateTimeProvider;


    private readonly ProductController _controller;
    private readonly OrderController _orderController;


    public IStrategyTests()
    {

        // Initialisera fakes
        _unitOfWork = A.Fake<IUnitOfWork>();
        _productRepository = A.Fake<IRepository<Product>>();
        _orderRepository = A.Fake<IRepository<Order>>();
        _customerRepository = A.Fake<IRepository<Customer>>();

        _discountStrategy = A.Fake<IDiscountStrategy>();
        _discountContext = A.Fake<DiscountContext>();
        _discountStrategyFactory = A.Fake<DiscountStrategyFactory>();
        _blackFridayDiscountStrategy = A.Fake<BlackFridayDiscountStrategy>();
        _noDiscountStrategy = A.Fake<NoDiscountStrategy>();
        _dateTimeProvider = A.Fake<DateTimeProvider>();

        // Initialisera controller
        _controller = new ProductController(_unitOfWork, _discountContext, _discountStrategyFactory, _dateTimeProvider);
        _orderController = new OrderController(_unitOfWork, _discountContext, _discountStrategyFactory, _dateTimeProvider);
    }

    


   



    #region TestCaseForOrderControllerWithDiscountStrategy

    [Fact]
    public async Task AddOrder_WithBlackFridayDiscount_ReturnsWithProductsOnDiscount()
    {
        // Arrange
        Order order = new Order
        {
            Id = 289,
            CustomerId = 39,
            Customer = new Customer
            {
                Id = 39,
                FirstName = "Johnny",
                LastName = "Bravo",
                Email = "Bravo@Cartoon.Com",
                Password = "DaxWax",
                Orders = null
            },
            OrderProducts = new List<OrderItem>()
            {
                new OrderItem
                {
                    OrderId = 289,
                    Order = null,
                    ProductId = 3,
                    Product = new Product
                    {
                        Id = 3,
                        Name = "Tvål",
                        Price = 100,
                        Stock = 70,
                        OrderProducts = null
                    },
                    Quantity = 1
                }
            },
            Quantity = 1
        };
        Product productInOrder = new Product
        {
            Id = 3,
            Name = "Tvål",
            Price = 100,
            Stock = 70,
            OrderProducts = null
        };
        var productWithoutDiscount = productInOrder.Price;

        A.CallTo(() => _unitOfWork.Repository<Order>()).Returns(_orderRepository);
        A.CallTo(() => _unitOfWork.Repository<Customer>()).Returns(_customerRepository);
        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _customerRepository.GetByIdAsync(order.CustomerId)).Returns(order.Customer);
        A.CallTo(() => _productRepository.GetByIdAsync(productInOrder.Id)).Returns(productInOrder);
        var blackFriday = new DateTime(2024, 11, 29);//BlackFriday
        A.CallTo(() => _dateTimeProvider.Now).Returns(blackFriday);//Gör så att testet får rätt datum.
        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(_dateTimeProvider.Now)).Returns(_discountStrategy); // Hämtar BlackFridayDiscount
        A.CallTo(() => _discountContext.GetDiscountPeriod(productInOrder)).Returns(50); // Returnerar priset med rabatt

        // Act
        var result = await _orderController.AddOrder(order);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var orderWithDiscountedProducts = Assert.IsAssignableFrom<Order>(okObjectResult.Value);
        Assert.NotEqual(productWithoutDiscount, orderWithDiscountedProducts.OrderProducts.First().Product.Price);

        A.CallTo(() => _customerRepository.GetByIdAsync(order.CustomerId)).MustHaveHappened();
        A.CallTo(() => _productRepository.GetByIdAsync(productInOrder.Id)).MustHaveHappened();
        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(_dateTimeProvider.Now)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _discountContext.GetDiscountPeriod(productInOrder)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitOfWork.CompleteAsync()).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region TestCasesForProductControllerWithDiscountStrategy

    [Fact]
    public async Task GetProductById_WithBlackFridayDiscount_ReturnsDiscountedProduct()
    {
        // Arrange
        var product = new Product
        {
            Id = 4,
            Name = "Tvål",
            Price = 100,
            Stock = 23,
            OrderProducts = null
        };
        var productPriceBeforeDiscount = product.Price;

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _productRepository.GetByIdAsync(4)).Returns(Task.FromResult(product));
        var blackFriday = new DateTime(2024, 11, 29);//BlackFriday
        A.CallTo(() => _dateTimeProvider.Now).Returns(blackFriday);//Gör så att testet får rätt datum.
        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(_dateTimeProvider.Now)).Returns(_discountStrategy); // Hämtar BlackFridayDiscount
        A.CallTo(() => _discountContext.GetDiscountPeriod(product)).Returns(50); // Returnerar priset med rabatt

        // Act
        var result = await _controller.GetProductById(4);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var productWithDiscount = Assert.IsAssignableFrom<Product>(okResult.Value);

        Assert.NotEqual(productPriceBeforeDiscount, productWithDiscount.Price);
        A.CallTo(() => _productRepository.GetByIdAsync(4)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(_dateTimeProvider.Now)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _discountContext.GetDiscountPeriod(product)).MustHaveHappenedOnceExactly();

    }
    [Fact]
    public async Task GetAllProducts_WithBlackFridayDiscount_ReturnsOk()
    {
        // Arrange

        List<Product> productList = new List<Product>()
        {
            new Product()
            {
                Id = 4,
                Name = "Tvål",
                Price = 100,
                Stock = 23,
                OrderProducts = null
            },
            new Product()
            {
                Id = 4,
                Name = "TvålBorste",
                Price = 100,
                Stock = 23,
                OrderProducts = null
            }

        };

        var product = productList.First();
        var productPriceBeforeDiscount = product.Price;


        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _productRepository.GetAllAsync()).Returns(Task.FromResult((IEnumerable<Product>)productList));

        var blackFriday = new DateTime(2024, 11, 29);//BlackFriday
        A.CallTo(() => _dateTimeProvider.Now).Returns(blackFriday);//Gör så att testet får rätt datum.

        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(_dateTimeProvider.Now)).Returns(_discountStrategy);
        A.CallTo(() => _discountContext.SetDiscountStrategy(_discountStrategy));
        A.CallTo(() => _discountContext.GetDiscountPeriod(product)).Returns(50);

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var productsWithDiscount = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

        Assert.Equal(2, productsWithDiscount.Count());

        Assert.NotEqual(productPriceBeforeDiscount, productsWithDiscount.First().Price);

        A.CallTo(() => _productRepository.GetAllAsync()).MustHaveHappened();
        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(_dateTimeProvider.Now)).MustHaveHappened();
        A.CallTo(() => _discountContext.SetDiscountStrategy(_discountStrategy)).MustHaveHappened();
        A.CallTo(() => _discountContext.GetDiscountPeriod(product)).MustHaveHappened();

    }


    #endregion

    #region DiscountStrategyFactoryTests
    [Fact]
    public async Task GetDiscountStrategy_OnBlackFriday_ReturnsBlackFridayDiscount()
    {
        //Arrange

        var product = new Product
        {
            Id = 4,
            Name = "Tvål",
            Price = 100,
            Stock = 23,
            OrderProducts = null
        };

        var blackFriday = new DateTime(2024, 11, 29);

        A.CallTo(() => _blackFridayDiscountStrategy.CalculatePrice(product)).Returns(50);
        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(blackFriday)).Returns(_blackFridayDiscountStrategy);

        //Act
        var resultStrategy = _discountStrategyFactory.GetDiscountStrategy(blackFriday);
        var resultPrice = resultStrategy.CalculatePrice(product);

        //Assert
        var actionResult = Assert.IsAssignableFrom<IDiscountStrategy>(resultStrategy);
        Assert.IsAssignableFrom<BlackFridayDiscountStrategy>(actionResult);
        Assert.Equal(50, resultPrice);

        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(blackFriday)).MustHaveHappened();
        A.CallTo(() => _blackFridayDiscountStrategy.CalculatePrice(product)).MustHaveHappened();
    }

    [Fact]
    public async Task GetDiscountStrategy_NoDiscount_ReturnsNoDiscount()
    {//Arrange

        var product = new Product
        {
            Id = 4,
            Name = "Tvål",
            Price = 100,
            Stock = 23,
            OrderProducts = null
        };

        var currenTime = DateTime.Now;
        A.CallTo(() => _noDiscountStrategy.CalculatePrice(product)).Returns(product.Price);
        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(currenTime)).Returns(_noDiscountStrategy);

        //Act
        var resultStrategy = _discountStrategyFactory.GetDiscountStrategy(currenTime);
        var resultPrice = resultStrategy.CalculatePrice(product);

        //Assert
        var actionResult = Assert.IsAssignableFrom<IDiscountStrategy>(resultStrategy);
        Assert.IsAssignableFrom<NoDiscountStrategy>(actionResult);
        Assert.Equal(product.Price, resultPrice);

        A.CallTo(() => _discountStrategyFactory.GetDiscountStrategy(currenTime)).MustHaveHappened();
        A.CallTo(() => _noDiscountStrategy.CalculatePrice(product)).MustHaveHappened();
    }


    #endregion

}