using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.Shared.Interfaces;

namespace WebShopTests.RepositoryTest;

public class CustomerRepositoryTests
{
    private readonly MyDbContext _InMemoryContext;
    private readonly IRepository<Customer> _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>().UseInMemoryDatabase(databaseName: "MyTestDatabaseCustomer")
            .Options;

        _InMemoryContext = new MyDbContext(options);

        _repository = new Repository<Customer>(_InMemoryContext);
    }


    [Fact]
    public async Task GetByIdAsync_ReturnsNull()
    {
        // Arrange
        Customer customer = new Customer
        {
            Id = 9,
            FirstName = "Johnny",
            LastName = "Bravo",
            Email = "Bravo@CartoonNetwork.com",
            Password = "DaxWax",
            Orders = null
        };

        await _InMemoryContext.AddAsync(customer);
        await _InMemoryContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(377);
        // Assert

        Assert.Null(result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }
    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectCustomer()
    {
        // Arrange
        Customer customer = new Customer
        {
            Id = 3,
            FirstName = "Johnny",
            LastName = "Bravo",
            Email = "Bravo@CartoonNetwork.com",
            Password = "DaxWax",
            Orders = null
        };

        await _InMemoryContext.AddAsync(customer);
        await _InMemoryContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(3);
        // Assert

        Assert.Equal(customer.Id, result.Id);
        Assert.Equal(customer.FirstName, result.FirstName);
        Assert.Equal(customer.LastName, result.LastName);
        Assert.Equal(customer.Email, result.Email);
        Assert.Equal(customer.Password, result.Password);

        Assert.Equal(customer, result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }
    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomers()
    {
        // Arrange
        Customer Johnny = new Customer
        {
            Id = 36,
            FirstName = "Johnny",
            LastName = "Bravo",
            Email = "Bravo@CartoonNetwork.com",
            Password = "DaxWax",
            Orders = null
        };
        Customer Samuraj = new Customer
        {
            Id = 38,
            FirstName = "Samuraj",
            LastName = "Jack",
            Email = "Samuraj@CartoonNetwork.com",
            Password = "DaxWax3",
            Orders = null
        };

        await _InMemoryContext.AddAsync(Johnny);
        await _InMemoryContext.AddAsync(Samuraj);
        await _InMemoryContext.SaveChangesAsync();
        // Act

        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(new List<Customer> { Johnny, Samuraj }, result);

        await _InMemoryContext.Database.EnsureDeletedAsync();

    }
    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList()
    {
        // Arrange
        Customer Johnny = new Customer
        {
            Id = 3,
            FirstName = "Johnny",
            LastName = "Bravo",
            Email = "Bravo@CartoonNetwork.com",
            Password = "DaxWax",
            Orders = null
        };

        Customer Samuraj = new Customer
        {
            Id = 366,
            FirstName = "Samuraj",
            LastName = "Jack",
            Email = "Samuraj@CartoonNetwork.com",
            Password = "DaxWax3",
            Orders = null
        };
        
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal([], result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }
    [Fact]
    public async Task AddAsync_AddsCustomer_ReturnsTrue()
    {
        Customer Johnny = new Customer
        {
            Id = 3,
            FirstName = "Johnny",
            LastName = "Bravo",
            Email = "Bravo@CartoonNetwork.com",
            Password = "DaxWax",
            Orders = null
        };

        // Act
        var result = await _repository.AddAsync(Johnny);

        // Assert
        Assert.True(result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }
    [Fact]
    public async Task AddAsync_AddsCustomer_ReturnsFalse()
    {
        // Arrange
        Customer customer = null;

        // Act
        var result = await _repository.AddAsync(customer);

        // Assert
        Assert.False(result);


    }
    [Fact]
    public async Task UpdateAsync_UpdatesCustomer_ReturnsTrue_UpdatedCustomer()
    {
        //Arrange
        Customer Fredrika = new Customer
        {
            Id = 3999,
            FirstName = "Fredrika",
            LastName = "Bravo",
            Email = "Bravo@CartoonNetwork.com",
            Password = "DaxWax",
            Orders = null
        };

        var customerNameBeforeUpdate = "Fredrika";

        await _InMemoryContext.AddAsync(Fredrika);
        await _InMemoryContext.SaveChangesAsync();

        var customerToUpdate = await _repository.GetByIdAsync(Fredrika.Id);

        customerToUpdate.FirstName = "Lise-Lotta";

        //Act
        await _repository.UpdateAsync(customerToUpdate);
        await _InMemoryContext.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(customerToUpdate.Id);

        //Assert
        Assert.NotEqual(customerNameBeforeUpdate, result.FirstName);

        Assert.True(true);

        await _InMemoryContext.Database.EnsureDeletedAsync();

    }
    [Fact]//Pga inbyggd EntityFramework funktionalitet så kan jag inte testa Update en fullskalig metod.
    public async Task UpdateAsync_UpdatesCustomer_ReturnsNull()
    {
        // Arrange
        Customer? customer = null;

        // Act
        var result = await _repository.UpdateAsync(customer);

        // Assert
        Assert.Null(result);
    }
    [Fact]
    public async Task RemoveAsync_RemoveCustomer_ReturnsTrue()
    {
        //Arrange
        Customer customer = new Customer
        {
            Id = 99,
            FirstName = "Fredrika",
            LastName = "Bravo",
            Email = "Bravo@CartoonNetwork.com",
            Password = "DaxWax",
            Orders = null
        };

        await _InMemoryContext.AddAsync(customer);
        await _InMemoryContext.SaveChangesAsync();

        //Act
        var result = await _repository.RemoveAsync(customer.Id);
        await _InMemoryContext.SaveChangesAsync();

        var checkIfCustomerExists = await _repository.GetByIdAsync(99);

        //Assert
        Assert.True(result);
        Assert.Null(checkIfCustomerExists);
        Assert.NotEqual(customer, checkIfCustomerExists);
        await _InMemoryContext.Database.EnsureDeletedAsync();
    }
    [Fact]
    public async Task RemoveAsync_RemoveCustomer_ReturnsFalse()
    {
        //Arrange
        Customer customer = new Customer
        {
            Id = 97,
            FirstName = "Fredrika",
            LastName = "Bravo",
            Email = "Bravo@CartoonNetwork.com",
            Password = "DaxWax",
            Orders = null
        };
        var fakeId = 23;
        await _InMemoryContext.AddAsync(customer);
        await _InMemoryContext.SaveChangesAsync();

        //Act
        var result = await _repository.RemoveAsync(fakeId);
        await _InMemoryContext.SaveChangesAsync();
        
        //Assert
        Assert.False(result);
   
        await _InMemoryContext.Database.EnsureDeletedAsync();
    }
}