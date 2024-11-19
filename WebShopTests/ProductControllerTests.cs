using Microsoft.AspNetCore.Mvc;
using FakeItEasy;
using Xunit;
using WebShop;
using WebShop.Controllers;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.Shared.Interfaces;


using WebShopSolution.DataAccess.Repositories.Products;
using WebShopSolution.DataAccess.UnitOfWork;
using System.Collections.Generic;

public class ProductControllerTests
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IProductRepository _productRepository = A.Fake<IProductRepository>();
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _productRepository = A.Fake<IProductRepository>();//Kolla om detta stämmer från övningsuppgifterna
        _unitOfWork = A.Fake<IUnitOfWork>();

        

        _controller = new ProductController(_unitOfWork);

    }

    [Fact]
    public async void GetProducts_ReturnsOkResult_WithAListOfProducts()
    {
        // Arrange
        var productListOfDummies = A.CollectionOfDummy<Product>(3);


        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);

        A.CallTo(() => _productRepository.GetAllAsync())
            .Returns(Task.FromResult((IEnumerable<Product>)productListOfDummies));
        
        // Act
        var result = await _controller.GetAllProducts();


        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
        var okResult = actionResult.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnValue = Assert.IsType<List<Product>>(okResult.Value);

        Assert.Equal(productListOfDummies, returnValue);
        Assert.Equal(3, returnValue.Count);

        A.CallTo(() => _productRepository.GetAllAsync()).MustHaveHappened();

    }

    [Fact]
    public void AddProduct_ReturnsOkResult()
    {
        // Arrange
        var product = A.Dummy<Product>();

        // Act
        _productRepository.AddAsync(product);
        // Assert
        A.CallTo(() => _productRepository.AddAsync(product)).MustHaveHappened();
    }
}
