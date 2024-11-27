using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using WebShop.Controllers;
using WebShopSolution.DataAccess.Entities;

using WebShopSolution.DataAccess.UnitOfWork;
using WebShopSolution.Shared.Interfaces;

namespace WebShopTests.ControllerTest;

public class OrderControllerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Order> _orderRepository;
    private readonly OrderController _orderController;


    public OrderControllerTests()
    {
        _orderRepository = A.Fake<IRepository<Order>>();
        _unitOfWork = A.Fake<IUnitOfWork>();

        _orderController = new OrderController(_unitOfWork);
    }
    
    #region DeleteOrder

    [Fact]
    public async Task DeleteOrder_WithInvalidInput_ReturnsNotFound404()
    {
        //Arrange
        A.CallTo(() => _unitOfWork.Repository<Order>()).Returns(_orderRepository);
        A.CallTo(() => _orderRepository.GetByIdAsync(58)).Returns(Task.FromResult<Order>(null));

        //Act
        var result = await _orderController.DeleteOrder(58);

        //Assert
        var notFound = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, notFound.StatusCode);

        A.CallTo(() => _orderRepository.GetByIdAsync(58)).MustHaveHappened();
        A.CallTo(() => _orderRepository.RemoveAsync(58)).MustNotHaveHappened();

    }

    [Fact]
    public async Task DeleteOrder_WithValidInput_ReturnsOK()
    {
        //Arrange
        Order order = new Order
        {
            Id = 2,
            CustomerId = 23,
            Customer = new Customer
            {
                Id = 23,
                FirstName = "Mikael",
                LastName = "Blazor",
                Email = "haha@Live.se",
                Password = "Hej123",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 0
        };
        A.CallTo(() => _unitOfWork.Repository<Order>()).Returns(_orderRepository);
        A.CallTo(() => _orderRepository.GetByIdAsync(58)).Returns(Task.FromResult<Order>(null));

        //Act

        var result = await _orderController.DeleteOrder(order.Id);
        //Assert
        Assert.IsType<OkResult>(result);
        A.CallTo(() => _orderRepository.GetByIdAsync(order.Id)).MustHaveHappened();
        A.CallTo(() => _orderRepository.RemoveAsync(order.Id)).MustHaveHappened();

    }

    #endregion

    #region GetOrderById
    [Fact]
    public async Task GetOrderById_WithInvalidInputAsNull_ReturnsNotFound()
    {
        //Arrange

        A.CallTo(() => _unitOfWork.Repository<Order>()).Returns(_orderRepository);
        A.CallTo(() => _orderRepository.GetByIdAsync(A<int>._)).Returns(Task.FromResult<Order>(null));


        //Act
        var result = await _orderController.GetOrderById(1);
        //Assert

        var actionResult = Assert.IsType<ActionResult<Order>>(result);
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);


        Assert.Equal(404, notFoundResult.StatusCode);
    }



    [Fact]
    public async Task GetOrderById_WithValidInput_ReturnOrder()
    {
        //Arrange
        Order order = new Order
        {
            Id = 2,
            CustomerId = 23,
            Customer = new Customer
            {
                Id = 23,
                FirstName = "Mikael",
                LastName = "Blazor",
                Email = "haha@Live.se",
                Password = "Hej123",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 0
        };

        A.CallTo(() => _unitOfWork.Repository<Order>()).Returns(_orderRepository);
        A.CallTo(() => _orderRepository.GetByIdAsync(order.Id)).Returns(Task.FromResult(order));

        await _orderController.AddOrder(order);

        //Act

        var result = await _orderController.GetOrderById(order.Id);

        //Assert

        var actionResult = Assert.IsType<ActionResult<Order>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

        var orderResult = Assert.IsAssignableFrom<Order>(okResult.Value);

        Assert.Equal(2, orderResult.Id);

        A.CallTo(() => _orderRepository.GetByIdAsync(order.Id)).MustHaveHappened();
    }


    #endregion

    #region GetAllOrders

    [Fact]
    public async Task GetAllOrders_ReturnNotFound()
    {
        //Arrange

        Order order = new Order
        {

            CustomerId = 4,
            Customer = new Customer(),
            OrderProducts = new List<OrderItem>(),
            Quantity = 0
        };

        await _orderController.AddOrder(order);

        //Act

        var result = await _orderController.GetAllOrders();
        //Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Order>>>(result);
        var notFoundObject = Assert.IsType<NotFoundResult>(actionResult.Result);
        Assert.Equal(404, notFoundObject.StatusCode);
    }

    [Fact]
    public async Task GetAllOrders_ReturnOk_WithFakeListOfOrders()
    {
        //Arrange

        var fakeOrders = A.CollectionOfDummy<Order>(5);

        A.CallTo(() => _unitOfWork.Repository<Order>()).Returns(_orderRepository);
        A.CallTo(() => _orderRepository.GetAllAsync())
            .Returns(Task.FromResult((IEnumerable<Order>)(fakeOrders)));


        //Act
        var result = await _orderController.GetAllOrders();

        //Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Order>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var orderList = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);

        Assert.NotEmpty(orderList);
        Assert.True(orderList.Any());
        Assert.Equal(5, orderList.Count());

        A.CallTo(() => _orderRepository.GetAllAsync()).MustHaveHappened();

    }

    #endregion

    #region AddOrder
    [Fact]
    public async Task AddOrder_WithValidInput_ReturnOk()
    {
        //Arrange
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
            OrderProducts = new List<OrderItem>(),
            Quantity = 2
        };
        //Act

        var result = await _orderController.AddOrder(order);

        //Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, actionResult.StatusCode);
    }

    [Fact]
    public async Task AddOrder_WithInValidInput_BadRequest()
    {
        //Arrange
        Order order = new Order
        {

            CustomerId = 39,
            Customer = new Customer
            {
                Id = 39,
                FirstName = "",
                LastName = "Bravo",
                Email = "Bravo@Cartoon.Com",
                Password = "DaxWax",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 2
        };

        _orderController.ModelState.AddModelError("FirstName", "Customer name are Required.");


        //Act

        var result = await _orderController.AddOrder(order);

        //Assert
        var actionResult = Assert.IsType<BadRequestResult>(result);

        Assert.Equal(400, actionResult.StatusCode);
    }


    #endregion

}