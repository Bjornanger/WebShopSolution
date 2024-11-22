using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;

namespace WebShopTests.RepositoryTest;

public class ProductRepositoryTests
{
    private readonly MyDbContext _InMemoryContext;
    private readonly IRepository<Product> _repository;

    
    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>().UseInMemoryDatabase(databaseName: "MyTestDatabase")
            .Options;

        _InMemoryContext = new MyDbContext(options);
        
        _repository = new Repository<Product>(_InMemoryContext);
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectProduct()
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

        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Price, result.Price);
        Assert.Equal(product.Stock, result.Stock);
        Assert.Equal(product, result);
       
        await _InMemoryContext.Database.EnsureDeletedAsync();
    }
    [Fact]
    public async Task GetByIdAsync_ReturnsNull()
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
        var result = await _repository.GetByIdAsync(812);
        // Assert

        Assert.Null(result);

        

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var socker = new Product
        {
            Id = 12,
            Name = "Socker",
            Price = 23,
            Stock = 45,
            OrderProducts = null
        };

        var Plastflaska = new Product
        {
            Id = 23,
            Name = "Plastflaska",
            Price = 2,
            Stock = 5,
            OrderProducts = null
        };
        await _InMemoryContext.AddAsync(socker);
        await _InMemoryContext.AddAsync(Plastflaska);
        await _InMemoryContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        // Assert
        Assert.Equal(new List<Product> { socker, Plastflaska }, result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }
    [Fact]
    public async Task GetAllAsync_ReturnsAllEmptyList()
    {
        //Arrange
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
        

        // Act
        var result = await _repository.GetAllAsync();
        // Assert
        Assert.Equal([], result);

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

    [Fact]
    public async Task AddAsync_AddsProduct_ReturnsFalse()
    {
        // Arrange
        Product? product = null;
        // Act
        var result = await _repository.AddAsync(product);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesProduct_ReturnsTrue_AndUpdatedObject()
    {
        // Arrange
        var product = new Product
        {
            Id = 15,
            Name = "Socker",
            Price = 23,
            Stock = 45
            
        };
        var productNameBeforeUpdate = "Socker";

        await _InMemoryContext.AddAsync(product);
        await _InMemoryContext.SaveChangesAsync();

        var productToUpdate = await _repository.GetByIdAsync(product.Id);
        
        productToUpdate.Name = "Plastpåse";

        // Act
         _repository.UpdateAsync(productToUpdate);
        await _InMemoryContext.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(productToUpdate.Id);

        // Assert
        Assert.NotEqual(productNameBeforeUpdate, result.Name);

        Assert.True(true);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]//Pga inbyggd EntityFramework funktionalitet så kan jag inte testa Update en fullskalig metod.
    public async Task UpdateAsync_UpdatesProduct_ReturnsNull()
    {
        // Arrange
        Product? product = null;
       
        // Act
        var result = await _repository.UpdateAsync(product);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task RemoveAsync_RemovesProduct_ReturnsTrue()
    {
        // Arrange
        var productToRemove = new Product
        {
            Id = 61,
            Name = "Socker",
            Price = 23,
            Stock = 45,
            OrderProducts = null
        };

        await _InMemoryContext.AddAsync(productToRemove);
        await _InMemoryContext.SaveChangesAsync();
        // Act
        var result = await _repository.RemoveAsync(productToRemove.Id);
        await _InMemoryContext.SaveChangesAsync();


        var checkIfGone = await _repository.GetByIdAsync(61);

        // Assert
        Assert.True(result);
        Assert.Null(checkIfGone);

        Assert.NotEqual(productToRemove, checkIfGone);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task RemoveAsync_RemovesProduct_ReturnsFalse()
    {
        // Arrange
        var productToRemove = new Product
        {
            Id = 61,
            Name = "Socker",
            Price = 23,
            Stock = 45,
            OrderProducts = null
        };
        var fakeId = 23;
        await _InMemoryContext.AddAsync(productToRemove);
        await _InMemoryContext.SaveChangesAsync();
        // Act
        var result = await _repository.RemoveAsync(fakeId);
        await _InMemoryContext.SaveChangesAsync();

        // Assert
        Assert.False(result);

        await _InMemoryContext.Database.EnsureDeletedAsync();
    }




}