using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using WebShop.Controllers;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories.Orders;
using WebShopSolution.DataAccess.UnitOfWork;

namespace WebShopTests.ControllerTest;

public class OrderControllerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly OrderController _orderController;


    public OrderControllerTests()
    {
        _orderRepository = A.Fake<IOrderRepository>();
        _unitOfWork = A.Fake<IUnitOfWork>();

        _orderController = new OrderController(_unitOfWork);
    }


    //Arrange
    //Act
    //Assert

    [Fact]
    public async Task GetOrderById_ReturnOrder()
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

        A.CallTo(()=> _unitOfWork.Repository<Order>()).Returns(_orderRepository);
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