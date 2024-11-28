using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using WebShop.Controllers;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;


using WebShopSolution.DataAccess.UnitOfWork;
using WebShopSolution.Shared.Interfaces;

namespace WebShopTests.ControllerTest;

public class CustomerControllerTests
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Customer> _customerRepository;
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _customerRepository = A.Fake<IRepository<Customer>>();
        _unitOfWork = A.Fake<IUnitOfWork>();

        _controller = new CustomerController(_unitOfWork);
    }

    #region DeleteCustomer
    [Fact]
    public async Task DeleteCustomer_WithInValidInput_ReturnsNotFound404()
    {
        //Arrange
        A.CallTo(() => _unitOfWork.Repository<Customer>()).Returns(_customerRepository);
        A.CallTo(() => _customerRepository.GetByIdAsync(56)).Returns(Task.FromResult<Customer>(null));

        //Act
        var result = await _controller.DeleteCustomer(56);

        //Assert
        var NotFoundObject = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, NotFoundObject.StatusCode);

        A.CallTo(() => _customerRepository.GetByIdAsync(56)).MustHaveHappened();
        A.CallTo(() => _customerRepository.RemoveAsync(56)).MustNotHaveHappened();
    }
    [Fact]
    public async Task DeleteCustomer_WithValidInput_ReturnsOk200()
    {
        //Arrange
        Customer customer = new Customer()
        {
            Id = 3,
            FirstName = "Kalle",
            LastName = "Anka",
            Email = "Ankeborg@Blazor.com",
            Password = "Hejsan123"
        };
        A.CallTo(() => _unitOfWork.Repository<Customer>()).Returns(_customerRepository);
        A.CallTo(() => _customerRepository.GetByIdAsync(customer.Id)).Returns(customer);
        
        //Act
        var result = await _controller.DeleteCustomer(customer.Id);

        //Assert
        var okObject = Assert.IsType<ObjectResult>(result);
        Assert.True(true);
        Assert.Equal(200, okObject.StatusCode);
        Assert.Equal("Ok", okObject.Value);
        A.CallTo(() => _customerRepository.RemoveAsync(customer.Id)).MustHaveHappened();
    }
    #endregion

    #region UpdateCustomer
    [Fact]
    public async Task UpdateCustomer_ModelStateIsInvalid_ReturnBadRequest400()
    {
        //Arrange
        Customer customer = new Customer()
        {
            FirstName = "",
            LastName = "Anka",
            Email = "Ankeborg@Blazor.com",
            Password = "Hejsan123"
        };
        _controller.ModelState.AddModelError("Name", "Name is required");

        //Act
        var result = await _controller.UpdateCustomer(1, customer);

        //Assert
        Assert.False(false);
        var actionResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
    }
    [Fact]
    public async Task UpdateCustomer_WithValidInput_ReturnsOkResult()
    {
        //Arrange
        Customer customer = new Customer()
        {
            Id = 2,
            FirstName = "Kalle",
            LastName = "Anka",
            Email = "Ankeborg@Blazor.com",
            Password = "Hejsan123"
        };
        await _controller.AddCustomer(customer);

        Customer customerToUpdate = new Customer()
        {
            Id = 2,
            FirstName = "Musse",
            LastName = "Pigg",
            Email = "Ankeborg@Blazor.com",
            Password = "Hejsan123"
        };
        A.CallTo(() => _unitOfWork.Repository<Customer>()).Returns(_customerRepository);
        A.CallTo(() => _customerRepository.GetByIdAsync(customer.Id)).Returns(Task.FromResult(customer));
        A.CallTo(() => _customerRepository.UpdateAsync(customer)).Returns(customerToUpdate);

        //Act
        var result = await _controller.UpdateCustomer(customer.Id, customerToUpdate);
        var response = await _controller.GetCustomerById(customer.Id);

        //Assert
        var OkResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, OkResult.StatusCode);
        Assert.True(true);
        var getResponse = Assert.IsType<OkObjectResult>(response.Result);
        var customerResponse = Assert.IsType<Customer>(getResponse.Value);

        Assert.NotEqual("Kalle", customerResponse.FirstName);
        Assert.NotEqual("Anka", customerResponse.LastName);
        Assert.Equal("Ankeborg@Blazor.com", customerResponse.Email);
        Assert.Equal("Hejsan123", customerResponse.Password);
        
        Assert.Equal("Musse", customerResponse.FirstName);
        Assert.Equal("Pigg", customerResponse.LastName);
        Assert.Equal("Ankeborg@Blazor.com", customerResponse.Email);
        Assert.Equal("Hejsan123", customerResponse.Password);
        A.CallTo(() => _customerRepository.UpdateAsync(customer)).MustHaveHappened();
    }
    #endregion

    #region GetCustomerById
    [Fact]
    public async Task GetCustomerById_WithInValidInputAsNull_ReturnNotFound()
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
        A.CallTo(() => _customerRepository.GetByIdAsync(A<int>._)).MustHaveHappened();
    }
    [Fact]
    public async Task GetCustomerById_WithValidInput_ReturnOk()
    {
        //Arrange
        var dummyCustomer = A.Dummy<Customer>();
        Customer customer = new Customer()
        {
            Id = 3,
            FirstName = "Kalle",
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
        A.CallTo(() => _customerRepository.GetByIdAsync(dummyCustomer.Id)).MustHaveHappened();
    }
    #endregion

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
        A.CallTo(() => _customerRepository.GetAllAsync()).MustHaveHappened();
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