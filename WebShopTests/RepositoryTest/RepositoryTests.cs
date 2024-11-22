using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;

namespace WebShopTests.RepositoryTest;

public class RepositoryTests
{
    private readonly MyDbContext _InMemoryContext;
    private readonly Repository<Product> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>().UseInMemoryDatabase(databaseName: "MyTestDatabase")
            .Options;

        _InMemoryContext = new MyDbContext(options);

        _repository = new Repository<Product>(_InMemoryContext);

        
        
    }

    //Arrange
    //Act
    //Assert

    [Fact]
    public async Task GetByIdAsync_ReturnsProduct()
    {
        // Arrange
        var product = new Product
        {
            Id = 8,
            Name = "Socker",
            Price = 23,
            Stock = 45,
            OrderProducts = null
        };
        await _InMemoryContext.AddAsync(product);
        await _InMemoryContext.SaveChangesAsync();
        // Act
        var result = await _repository.GetByIdAsync(8);
        // Assert
        Assert.Equal(product, result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var product1 = new Product
        {
            Id = 12,
            Name = "Socker",
            Price = 23,
            Stock = 45,
            OrderProducts = null
        };

        var product2 = new Product
        {
            Id = 23,
            Name = "Plastflaska",
            Price = 2,
            Stock = 5,
            OrderProducts = null
        };

        await _InMemoryContext.AddAsync(product1);
        await _InMemoryContext.AddAsync(product2);
        await _InMemoryContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        // Assert
        Assert.Equal(new List<Product> { product1, product2 }, result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task AddAsync_AddsProduct_ReturnsTrue()
    {
        // Arrange
        var product = new Product
        {
            Id = 41,
            Name = "Socker",
            Price = 23,
            Stock = 45,
            OrderProducts = null
        };
        // Act
        var result = await _repository.AddAsync(product);
        // Assert
        Assert.True(result);
        

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    //[Fact]
    //public async Task UpdateAsync_UpdatesProduct_ReturnsTrue_AndUpdatedObject()
    //{
    //    // Arrange
    //    var product = new Product
    //    {
    //        Id = 15,
    //        Name = "Socker",
    //        Price = 23,
    //        Stock = 45,
    //        OrderProducts = null
    //    };
    //    await _InMemoryContext.AddAsync(product);
    //    await _InMemoryContext.SaveChangesAsync();
    //    product.Name = "Plastpåse";


    //    // Act
    //    var result = await _repository.UpdateAsync(product);
    //    // Assert
    //    Assert.NotEqual(product, result);

    //    await _InMemoryContext.Database.EnsureDeletedAsync();
    //}

    //[Fact]
    //public async Task RemoveAsync_RemovesProduct()
    //{
    //    // Arrange
    //    var product = new Product
    //    {
    //        Id = 61,
    //        Name = "Socker",
    //        Price = 23,
    //        Stock = 45,
    //        OrderProducts = null
    //    };


    //    await _InMemoryContext.AddAsync(product);
    //    await _InMemoryContext.SaveChangesAsync();
    //    // Act
    //    var result = await _repository.RemoveAsync(product.Id);

    //    var checkIfGone = await _repository.GetByIdAsync(product.Id);

    //    // Assert
    //    Assert.True(result);
    //    Assert.NotEqual(product, checkIfGone);
        


    //    await _InMemoryContext.Database.EnsureDeletedAsync();
    //}





}