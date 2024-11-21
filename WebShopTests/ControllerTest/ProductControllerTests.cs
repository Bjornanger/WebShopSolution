using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using WebShop.Controllers;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories.Products;
using WebShopSolution.DataAccess.UnitOfWork;
using A = FakeItEasy.A;

namespace WebShopTests.ControllerTest;

public class ProductControllerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly ProductController _controller;
    
    public ProductControllerTests()
    {
       
        // Initialisera fakes
        _productRepository = A.Fake<IProductRepository>(); 
        _unitOfWork = A.Fake<IUnitOfWork>();
         
        
        // Initialisera controller
        _controller = new ProductController(_unitOfWork);

    }
    
    #region DeleteProducts

    [Fact]
    public async Task DeleteProduct_WithInvalidInput_ReturnNotFound()
    {
        //Arrange
        Product product = new Product
        {
            Id = 98,
            Name = "Socker",
            Price = 10,
            Stock = 20,
            OrderProducts = null
        };

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _productRepository.GetByIdAsync(product.Id)).Returns(Task.FromResult(product));
        A.CallTo(() => _productRepository.GetByIdAsync(345)).Returns(Task.FromResult<Product>(null));

        //Act
        var result = await _controller.DeleteProduct(345);


        //Assert
        var NotFoundObject = Assert.IsType<NotFoundResult>(result);

        Assert.False(false);
        Assert.Equal(404, NotFoundObject.StatusCode);
        Assert.False(false);
        A.CallTo(() => _productRepository.GetByIdAsync(345)).MustHaveHappened();
    }

    [Fact]
    public async Task DeleteProduct_WithValidInput_ReturnOk()
    {
        //Arrange
        Product product = new Product
        {
            Id = 1,
            Name = "Socker",
            Price = 10,
            Stock = 20,
            OrderProducts = null
        };

        _productRepository.AddAsync(product);

        //Act
        var result = await _controller.DeleteProduct(1);


        //Assert
        var okObject = Assert.IsType<ObjectResult>(result);
        Assert.True(true);
        Assert.Equal(200, okObject.StatusCode);
        Assert.Equal("Ok", okObject.Value);
    }
    #endregion

    #region UpdateProduct
    [Fact]
    public async Task UpdateProduct_WithValidInput_ReturnsOkResult()
    {
        //Arrange
        Product product = new Product
        {
            Id = 1,
            Name = "Socker",
            Price = 10,
            Stock = 20,
            OrderProducts = null
        };

        await _controller.AddProduct(product);


        Product productToUpdateValueOn = new Product
        {
            Id = 1,
            Name = "Fotkr�m",
            Price = 50,
            Stock = 60,
            OrderProducts = null
        };


        //Act

        var result = await _controller.UpdateProduct(1, productToUpdateValueOn);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var updatedProduct = Assert.IsAssignableFrom<Product>(okResult.Value);
        Assert.Equal(productToUpdateValueOn.Name, updatedProduct.Name);
        Assert.Equal(productToUpdateValueOn.Price, updatedProduct.Price);
        Assert.Equal(productToUpdateValueOn.Stock, updatedProduct.Stock);


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
    public async Task UpdateProduct_ModelStatWithNotValidInput_ReturnBadRequest()
    {
        //Arrange

        var productToUpdate = new Product
        {
            Id = 1,
            Name = "E",
            Price = 40,
            Stock = 60,
            OrderProducts = null
        };


        _controller.ModelState.AddModelError("Name", "Name is required");


        //Act
        var result = await _controller.UpdateProduct(1, productToUpdate);

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
       
        //Act
        var result = await _controller.GetAllProducts();


        //Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);

        Assert.Equal(404, notFoundResult.StatusCode);
    }
    
    [Fact]
    public async Task GetAllProducts_ReturnsOkResult_WithAListOfProducts()
    {
        // Arrange

        Product product = new Product
        {
           
            Name = "Socker",
            Price = 10,
            Stock = 20,
            OrderProducts = null
        };
        await _controller.AddProduct(product);



        // Act
        var result = await _controller.GetAllProducts();


        // Assert
        Assert.NotNull(result);
        Assert.IsType<ActionResult<IEnumerable<Product>>>(result);

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
    public async Task GetProductById_ProductIsNotFound_ReturnNotFound()
    {
        //Arrange
        
        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _productRepository.GetByIdAsync(A<int>._)).Returns(Task.FromResult<Product>(null));
        
        //Act
        var result = await _controller.GetProductById(34);
        
        //Assert
        
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var objectResult = Assert.IsType<NotFoundResult>(actionResult.Result);
        Assert.Equal(404, objectResult.StatusCode);
        A.CallTo(() => _productRepository.GetByIdAsync(A<int>._)).MustHaveHappened();
    }
    
    [Fact]
    public async Task GetProductsById_ReturnsOkResult_WithSpecificProducts()
    {
        // Arrange
        var product = A.Dummy<Product>();
        var expectedProduct = new Product
        {
            Id = product.Id,
            Name = "Socker",
            Price = 10,
            Stock = 20,
            OrderProducts = null

        }; 

        
        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _productRepository.GetByIdAsync(product.Id)).Returns(Task.FromResult(expectedProduct)); 
        
        // Act
        var result = await _controller.GetProductById(product.Id);


        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result); 
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.NotNull(result);
        Assert.Equal(expectedProduct.Id, ((Product)okResult.Value).Id);
        Assert.Equal(expectedProduct.Name, ((Product)okResult.Value).Name);
        A.CallTo(() => _productRepository.GetByIdAsync(product.Id)).MustHaveHappened();
    }


    #endregion

    #region AddProduct

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

        var product = new Product
        {

            Name = "T",
            Price = 0,
            Stock = 0,
            OrderProducts = null
        };
        _controller.ModelState.AddModelError("Name", "Name is required");


        //Act
        var result = await _controller.AddProduct(product);

        //Assert
        Assert.False(false);
        var actionResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
    }

    [Fact]
    public async void AddProduct_TestsTheMethodWithFakeItEasy_ReturnsOkResultAndTrue()
    {
        // Arrange
       
        var product = A.Dummy<Product>();

        A.CallTo(() => _unitOfWork.Repository<Product>()).Returns(_productRepository);
        A.CallTo(() => _productRepository.AddAsync(product)).Returns(true);

        // Act
        var result = await _controller.AddProduct(product);

        // Assert
        Assert.True(true);
        var actionResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, actionResult.StatusCode);
        A.CallTo(() => _unitOfWork.Repository<Product>()).MustHaveHappened();
        A.CallTo(() => _productRepository.AddAsync(product)).MustHaveHappened();

    }

    [Fact]
    public async Task AddProduct_ValidProduct_AddsProduct()
    {
        //Arrange
        var product = new Product
        {
            Name = "socker",
            Price = 3,
            Stock = 4,
            OrderProducts = null
        };
        
        //Act
        var result = await _controller.AddProduct(product);

        //Assert
        var actionResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, actionResult.StatusCode);
    }

    [Fact]
    public async Task AddProduct_WithInvalidInput_ReturnStatusCode500()
    {
        //Arrange
        
        var product = new Product
        {
            Name = "Ap",
            Price = 3,
            Stock = 4,
            OrderProducts = null
        };
        
        _controller.ModelState.AddModelError("Name", "Name is required");

        //Act
        var result = await _controller.AddProduct(product);

        //Assert
        var actionResult = Assert.IsAssignableFrom<BadRequestResult>(result);
        Assert.Equal(400, actionResult.StatusCode);

    }

    #endregion

}