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
using FakeItEasy.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using A = FakeItEasy.A;

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
    //Arrange
    //Act
    //Assert

    #region UpdateProduct
    [Fact]
    public async Task UpdateProduct_WithValidInput_ReturnsOkResult()
    {
        //Arrange
        var product = A.Dummy<Product>();

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);

        A.CallTo(() => _productRepository.GetByIdAsync(A<int>._)).Returns(Task.FromResult(product));
        A.CallTo(() => _productRepository.UpdateAsync(product)).Returns(Task.CompletedTask);
        A.CallTo(() => _unitOfWork.CompleteAsync()).Returns(Task.CompletedTask);


        //Act
        var result = await _controller.UpdateProduct(1, product);

        //Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, actionResult.StatusCode);


        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();
        A.CallTo(() => _productRepository.GetByIdAsync(A<int>._)).MustHaveHappened();
        A.CallTo(() => _productRepository.UpdateAsync(product)).MustHaveHappened();
        A.CallTo(() => _unitOfWork.CompleteAsync()).MustHaveHappenedOnceExactly();


    }

    [Fact]
    public async Task UpdateProduct_ThrowException_ActivateDisposeAndSendStatusCode500()
    {
        //Arrange
        var product = A.Dummy<Product>();


        A.CallTo(() => _productRepository.GetByIdAsync(A<int>._)).Returns(Task.FromResult(product));
        A.CallTo(() => _productRepository.UpdateAsync(product)).Throws(new Exception("Test"));
        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);

        //Act
        var result = await _controller.UpdateProduct(1, product);

        //Assert
        var objectResult = Assert.IsType<ObjectResult>(result);

        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Internal server error", objectResult.Value);

        A.CallTo(() => _unitOfWork.Dispose()).MustHaveHappened();

    }

    [Fact]
    public async Task UpdateProduct_ProductRepositoryNotFound_ReturnNotFound()
    {
        //Arrange
        var product = A.Dummy<Product>();

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(null as IProductRepository);

        //Act
        var result = await _controller.UpdateProduct(1, product);

        //Assert
        var actionResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, actionResult.StatusCode);

        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();
    }
    [Fact]
    public async Task UpdateProduct_ProductIsNull_ReturnBadRequest()
    {
        //Arrange
        Product product = null;

        //Act
        var result = await _controller.UpdateProduct(1, product);

        //Assert
        var actionResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
        Assert.Null(product);
    }


    [Fact]
    public async Task UpdateProduct_ModelStatNotValid_ReturnBadRequest()
    {
        //Arrange
        var product = new Product();

        _controller.ModelState.AddModelError("Name", "Name is required");


        //Act
        var result = await _controller.UpdateProduct(1, product);

        //Assert
        var actionResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(400, actionResult.StatusCode);

    }



    #endregion
    
    #region GetAllProducts

    [Fact]
    public async Task GetAllProducts_ThrowException_ActivateDisposeAndSendStatusCode500()
    {

        //Arrange

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _productRepository.GetAllAsync()).Throws(new Exception("Test"));

        //Act
        var result = await _controller.GetAllProducts();
        //Assert

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);

        Assert.NotNull(objectResult);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Internal server error", objectResult.Value);

        A.CallTo(() => _unitOfWork.Dispose()).MustHaveHappened();

    }

    [Fact]
    public async Task GetAllProducts_ProductListAreEmpty_ReturnNotFound()
    {
        //Arrange

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _productRepository.GetAllAsync()).Returns(Task.FromResult<IEnumerable<Product>>(null));

        //Act
        var result = await _controller.GetAllProducts();


        //Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);

        Assert.Equal(404, notFoundResult.StatusCode);

        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();

    }


    [Fact]
    public async Task GetAllProducts_ProductRepositoryNotFound_ReturnNotFound()
    {
        //Arrange

        var productList = A.CollectionOfDummy<Product>(5);

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(null as IProductRepository);

        //Act

        var result = await _controller.GetAllProducts();

        //Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);

        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);
        Assert.Equal(404, notFoundResult.StatusCode);

        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();

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



    #endregion
    
    #region GetProductById

    [Fact]
    public async Task GetProductById_WithInvalidInput_ActivateDisposeAndSendStatusCode500()
    {
        //Arrange

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _productRepository.GetByIdAsync(1)).Throws(new Exception("Test"));

        //Act
        var result = await _controller.GetProductById(1);

        //Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);

        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Internal server error", objectResult.Value);

        A.CallTo(() => _unitOfWork.Dispose()).MustHaveHappened();
    }
    
    [Fact]
    public async Task GetProductById_ProductIsNull_ReturnBadRequest()
    {
        //Arrange

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);

        A.CallTo(() => _productRepository.GetByIdAsync(A<int>._)).Returns(Task.FromResult<Product>(null));

        
        //Act
        var result = await _controller.GetProductById(1);

        //Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var objectResult = Assert.IsType<BadRequestResult>(actionResult.Result);
        Assert.Equal(400, objectResult.StatusCode);
        

        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();
    }

    [Fact]
    public async Task GetProductById_ProductRepositoryNotFound_ReturnNotFound()
    {
        // Arrange
        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(null as IProductRepository);

        // Act
        var result = await _controller.GetProductById(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);
        Assert.Equal(404, notFoundResult.StatusCode);

        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();
    }

    [Fact]
    public async Task GetProductsById_ReturnsOkResult_WithSpecificProducts()
    {
        //Arrange 

        var dummyProduct = A.Dummy<Product>();

        A.CallTo(() => _productRepository.GetByIdAsync(dummyProduct.Id)).Returns(Task.FromResult(dummyProduct));
        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        // Act
        var result = await _controller.GetProductById(dummyProduct.Id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        Assert.IsType<OkObjectResult>(actionResult.Result);


        A.CallTo(() => _productRepository.GetByIdAsync(dummyProduct.Id)).MustHaveHappened();
        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();


    }


    #endregion
    
    #region AddProduct

    [Fact]
    public async Task AddProduct_ProductRepositoryNotFound_ReturnNotFound()
    {
        //Arrange

        var product = A.Dummy<Product>();


        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(null as IProductRepository);


        //Act
        var result = await _controller.AddProduct(product);
        //Assert
        var actionResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, actionResult.StatusCode);

        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();

    }

    [Fact]
    public async Task AddProduct_ProductIsNull_ReturnBadRequest()
    {
        //Arrange
        Product product = null;


        //Act
        var result = await _controller.AddProduct(product);


        //Assert
        var actionResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
        Assert.Null(product);

    }

    [Fact]
    public async Task AddProduct_ModelStateIsInvalid_ReturnBadRequest()
    {
        //Arrange

        var product = new Product();
        _controller.ModelState.AddModelError("Name", "Name is required");


        //Act

        var result = await _controller.AddProduct(product);
        //Assert
        var actionResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
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
        var actionResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, actionResult.StatusCode);
        A.CallTo(() => _unitOfWork.CompleteAsync()).MustHaveHappened();
        A.CallTo(() => _productRepository.AddAsync(product)).MustHaveHappened();
    }

    [Fact]
    public async Task AddProduct_ValidProduct_AddsProduct()
    {
        //Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Socker",
            Price = 3,
            Stock = 4,
            OrderProducts = null
        };

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);

        A.CallTo(() => _productRepository.AddAsync(product)).Returns(Task.CompletedTask);



        //Act
        var result = await _controller.AddProduct(product);
        //Assert
        var actionResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, actionResult.StatusCode);



        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();
        A.CallTo(() => _productRepository.AddAsync(product)).MustHaveHappened();
        A.CallTo(() => _unitOfWork.CompleteAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task AddProduct_WithInvalidInput_ReturnStatusCode500()
    {
        //Arrange
        var product = A.Dummy<Product>();
       

        A.CallTo(() => _productRepository.AddAsync(product)).Throws(new Exception("Test"));
        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);

        //Act

        var result = await _controller.AddProduct(product);
        //Assert

        var actionResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, actionResult.StatusCode);
        Assert.Equal("Internal server error", actionResult.Value);

        A.CallTo(() => _unitOfWork.Dispose()).MustHaveHappened();

    }

    #endregion



}
