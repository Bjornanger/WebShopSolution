using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.Shared.Interfaces;

namespace WebShopTests.RepositoryTest;

public class OrderRepositoryTests
{
    private readonly MyDbContext _InMemoryContext;
    private readonly IRepository<Order> _repository;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>().UseInMemoryDatabase(databaseName: "MyTestDatabaseOrder")
            .Options;

        _InMemoryContext = new MyDbContext(options);

        _repository = new Repository<Order>(_InMemoryContext);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull()
    {
        // Arrange
        Order order = new Order
        {
            Id = 2,
            CustomerId = 3,
            Customer = null,
            OrderProducts = null,
            Quantity = 2
        };
        await _InMemoryContext.AddAsync(order);
        await _InMemoryContext.SaveChangesAsync();
        // Act
        var result = await _repository.GetByIdAsync(82);
        // Assert
        Assert.Null(result);
        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOrder()
    {
        // Arrange
        Order order = new Order
        {
            Id = 2,
            CustomerId = 2,
            Customer = new Customer
            {
                Id = 2,
                FirstName = "Johnny",
                LastName = "Bravo",
                Email = "Bravo@Cartoon.Com",
                Password = "DaxWax",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 2
        };
        await _InMemoryContext.AddAsync(order);
        await _InMemoryContext.SaveChangesAsync();
        // Act
        var result = await _repository.GetByIdAsync(2);
        // Assert
        Assert.Equal(order, result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        //Arrange
        Order order = new Order
        {
            Id = 2,
            CustomerId = 89,
            Customer = new Customer
            {
                Id = 89,
                FirstName = "Johnny",
                LastName = "Bravo",
                Email = "Bravo@Cartoon.Com",
                Password = "DaxWax",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 2
        };
        Order order2 = new Order
        {
            Id = 3,
            CustomerId = 93,
            Customer = new Customer
            {
                Id = 93,
                FirstName = "Fredrika",
                LastName = "Bravo",
                Email = "Bravo@Cartoon.Com",
                Password = "DaxWax",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 2
        };

        await _InMemoryContext.AddAsync(order);
        await _InMemoryContext.AddAsync(order2);
        await _InMemoryContext.SaveChangesAsync();

        //Act
        var result = await _repository.GetAllAsync();

        //Assert

        Assert.Equal(new List<Order> { order, order2 }, result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList()
    {

        //Arrange
        Order order = new Order
        {
            Id = 62,
            CustomerId = 849,
            Customer = new Customer
            {
                Id = 849,
                FirstName = "Johnny",
                LastName = "Bravo",
                Email = "Bravo@Cartoon.Com",
                Password = "DaxWax",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 2
        };
        Order order2 = new Order
        {
            Id = 63,
            CustomerId = 943,
            Customer = new Customer
            {
                Id = 943,
                FirstName = "Fredrika",
                LastName = "Bravo",
                Email = "Bravo@Cartoon.Com",
                Password = "DaxWax",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 2
        };

        await _InMemoryContext.AddAsync(order);
        await _InMemoryContext.AddAsync(order2);
        
        //Act
        var result = await _repository.GetAllAsync();

        //Assert
        Assert.Equal([], result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task AddAsync_ReturnsTrue()
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

        await _InMemoryContext.AddAsync(order);
        await _InMemoryContext.SaveChangesAsync();

        //Act

        var result = await _repository.AddAsync(order);
        //Assert
        Assert.True(result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task AddAsync_ReturnsFalse()
    {
        //Arrange
        Order order = null;

        //Act
        var result = await _repository.AddAsync(order);

        //Assert
        Assert.False(result);
        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesOrder_ReturnsTrue_AndUpdatedOrder()
    {
        //Arrange
        Order order = new Order
        {
            Id = 2890,
            CustomerId = 390,
            Customer = new Customer
            {
                Id = 390,
                FirstName = "Johnny",
                LastName = "Bravo",
                Email = "Bravo@Cartoon.Com",
                Password = "DaxWax",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 2
        };

        var orderQuantityBeforeUpdate = 2;
        await _InMemoryContext.AddAsync(order);
        await _InMemoryContext.SaveChangesAsync();

        var orderToUpdate = await _repository.GetByIdAsync(order.Id);

        orderToUpdate.Quantity = 3;

        //Act

        await _repository.UpdateAsync(orderToUpdate);
        await _InMemoryContext.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(orderToUpdate.Id);

        //Assert
        Assert.Equal(3, result.Quantity);
        Assert.True(true);
        Assert.NotEqual(orderQuantityBeforeUpdate, result.Quantity);

        await _InMemoryContext.Database.EnsureDeletedAsync();

    }

    [Fact]
    public async Task UpdateAsync_UpdatesOrder_ReturnsNull()
    {
        // Arrange
        Order? order = null;

        // Act
        var result = await _repository.UpdateAsync(order);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_RemoveOrder_ReturnsTrue()
    {
        //Arrange
        Order order = new Order
        {
            Id = 29,
            CustomerId = 359,
            Customer = new Customer
            {
                Id = 359,
                FirstName = "Johnny",
                LastName = "Bravo",
                Email = "Bravo@Cartoon.Com",
                Password = "DaxWax",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 2
        };

        await _InMemoryContext.AddAsync(order);
        await _InMemoryContext.SaveChangesAsync();
        //Act
        var result = await _repository.RemoveAsync(order.Id);
        await _InMemoryContext.SaveChangesAsync();

        var checkIfGone = await _repository.GetByIdAsync(29);

        //Assert
        Assert.True(result);
        Assert.Null(checkIfGone);
        Assert.NotEqual(order, checkIfGone);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task RemoveAsync_RemoveOrder_ReturnsFalse()
    {
        //Arrange
        Order order = new Order
        {
            Id = 29,
            CustomerId = 359,
            Customer = new Customer
            {
                Id = 359,
                FirstName = "Johnny",
                LastName = "Bravo",
                Email = "Bravo@Cartoon.Com",
                Password = "DaxWax",
                Orders = null
            },
            OrderProducts = new List<OrderItem>(),
            Quantity = 2
        };

        var fakeId = 78;

        await _InMemoryContext.AddAsync(order);
        await _InMemoryContext.SaveChangesAsync();
        //Act
        var result = await _repository.RemoveAsync(fakeId);
        await _InMemoryContext.SaveChangesAsync();

        //Assert
        Assert.False(result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }
}





