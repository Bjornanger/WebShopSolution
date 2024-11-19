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
using System.Diagnostics.CodeAnalysis;

public class ProductControllerTests
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IProductRepository _productRepository = A.Fake<IProductRepository>();
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        // Initialisera fakes
         _productRepository = A.Fake<IProductRepository>(); 
         _unitOfWork = A.Fake<IUnitOfWork>();
         
        
         // Initialisera controller
         _controller = new ProductController(_unitOfWork);

    }

    [Fact]
    public async Task GetAllProducts_ReturnsOkResult_WithAListOfProducts()
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


        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();
        A.CallTo(() => _productRepository.GetAllAsync()).MustHaveHappened();


    }

    [Fact]
    public async Task GetProductsById_ReturnsOkResult_WithSpecificProducts()
    {
        //Arrange 

        var dummyProduct = A.Dummy<Product>();
       
        A.CallTo(() => _productRepository.GetByIdAsync(dummyProduct.Id)).Returns(Task.FromResult(dummyProduct));
       
        // Act
        var result = await _controller.GetProductById(dummyProduct.Id); 
        
        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

        Assert.NotNull(okResult);

        var returnValue = Assert.IsAssignableFrom<Product>(okResult.Value);

        Assert.Equal(dummyProduct.Id, returnValue.Id);
        Assert.Equal(dummyProduct.Name, returnValue.Name); 
        Assert.Equal(dummyProduct.Price, returnValue.Price);
        Assert.Equal(dummyProduct.Stock, returnValue.Stock);
        
       //Kontroll på A.CallTo
        A.CallTo(() => _productRepository.GetByIdAsync(dummyProduct.Id)).MustHaveHappened(); 
       
    }

    [Fact]
    public async void AddProduct_ReturnsOkResult()
    {
        // Arrange
        var product = A.Dummy<Product>();

       
        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _productRepository.AddAsync(product));
        // Act

        var result = await _controller.AddProduct(product);
        
        // Assert
        Assert.IsType<OkResult>(result);
       


        A.CallTo(() => _unitOfWork.CompleteAsync()).MustHaveHappened();
        A.CallTo(() => _productRepository.AddAsync(product)).MustHaveHappened();
    }
}
