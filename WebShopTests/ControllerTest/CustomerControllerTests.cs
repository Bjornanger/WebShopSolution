using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using WebShop.Controllers;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.DataAccess.Repositories.Customer;
using WebShopSolution.DataAccess.Repositories.Products;
using WebShopSolution.DataAccess.UnitOfWork;

namespace WebShopTests.ControllerTest;

public class CustomerControllerTests
{
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerRepository _customerRepository;
    private readonly CustomerController _controller;


    private readonly MyDbContext _context;




    public CustomerControllerTests()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>().UseInMemoryDatabase(databaseName: "MyTestDatabase")
            .Options;

        _context = new MyDbContext(options);




        _customerRepository = A.Fake<ICustomerRepository>();
        _unitOfWork = A.Fake<IUnitOfWork>();

         _controller = new CustomerController(_unitOfWork);
    }

    //Arrange
    //Act
    //Assert

    [Fact]
    public async Task GetCustomerById_WithInValidInput_ReturnNotFound()
    {
        //Arrange
        A.CallTo(() => _unitOfWork.Repository<Customer>()).Returns(_customerRepository);
        A.CallTo(() => _customerRepository.GetByIdAsync(A<int>._)).Returns(Task.FromResult<Customer>(null));

        // //Act
        var result = await _controller.GetCustomerById(67);

        //Assert
        var actionResult = Assert.IsType<ActionResult<Customer>>(result);
        var notFoundObject = Assert.IsType<NotFoundResult>(actionResult.Result);
        Assert.Equal(404, notFoundObject.StatusCode);
        
    }

     

    [Fact]
    public async Task GetCustomerById_WithValidInput_ReturnOk()
    {
        //Arrange

        var dummyCustomer = A.Dummy<Customer>();


        Customer customer = new Customer()
        {
            Id = 3, FirstName = "Kalle",
            LastName = "Anka",
            Email = "Ankeborg@Blazor.com",
            Password = "Hejsan123"
        };
        A.CallTo(() => _unitOfWork.Repository<Customer>()).Returns(_customerRepository);
        A.CallTo(() => _customerRepository.GetByIdAsync(dummyCustomer.Id)).Returns(Task.FromResult(customer));
        
        // //Act
        var result = await _controller.GetCustomerById(dummyCustomer.Id);
        
        //Assert
        var actionResult = Assert.IsType<ActionResult<Customer>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

        var dummy = Assert.IsAssignableFrom<Customer>(okResult.Value);
        
        Assert.Equal(customer.Id, dummy.Id);
        Assert.Equal(customer.FirstName, dummy.FirstName);
        Assert.Equal(customer.LastName, dummy.LastName);
        Assert.Equal(customer.Email, dummy.Email);
    }


    #region GetAllCustomers

    [Fact]
    public async Task GetAllCustomers_ReturnOkResult_WIthAFakeListOFCustomers()
    {
        //Arrange
        var fakeCustomerLIst = A.CollectionOfDummy<Customer>(4);

        A.CallTo(() => _unitOfWork.Repository<Customer>()).Returns(_customerRepository);
        A.CallTo(() => _customerRepository.GetAllAsync()).
            Returns(Task.FromResult((IEnumerable<Customer>)(fakeCustomerLIst)));


        //Act

        var result = await _controller.GetAllCustomers();
        //Assert

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Customer>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var customerList = Assert.IsAssignableFrom<IEnumerable<Customer>>(okResult.Value);

        Assert.NotEmpty(customerList);
        Assert.True(customerList.Any());
        Assert.Equal(4, customerList.Count());


    }


    [Fact]
    public async Task GetAllCustomers_ReturnNotFound()
    {
        //Arrange

        Customer customer = new Customer()
        {
            FirstName = "Kalle",
            LastName = "Anka",
            Email = "aaa@haha.com",
            Password = "Hejsan123"
        };

       
        await _controller.AddCustomer(customer);

        //Act

        var result = await _controller.GetAllCustomers();


        //Assert

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Customer>>>(result);
        var notFoundObject = Assert.IsType<NotFoundResult>(actionResult.Result);
        Assert.Equal(404, notFoundObject.StatusCode);
    }

    #endregion
    
    #region AddCustomer

    [Fact]
    public async Task AddCustomer_WithValidInput_ReturnOK()
    {
        //Arrange
        Customer customer = new Customer()
        {

            FirstName = "Kalle",
            LastName = "Anka",
            Email = "Ankeborg@Blazor.com",
            Password = "Hejsan123"
        };
        

        //Act
        var result = await _controller.AddCustomer(customer);

        //Assert
        var actionResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, actionResult.StatusCode);
    }

    [Fact]
    public async Task AddCustomer_WithInValidInput_ReturnBadRequest()
    {
        //Arrange
        Customer customer = new Customer
        {

            FirstName = "",
            LastName = "Lallegren",
            Email = "aaa@aaa.com",
            Password = "Hej123",
            Orders = null
        };

        _controller.ModelState.AddModelError("Name", "Name are Required");

        //Act

        var result = await _controller.AddCustomer(customer);
        //Assert

        var actionResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
    }

    #endregion
    
}