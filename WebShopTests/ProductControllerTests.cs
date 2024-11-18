using Microsoft.AspNetCore.Mvc;
using FakeItEasy;
using Xunit;
using WebShop;
using WebShop.Controllers;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.Shared.Interfaces;


using WebShopSolution.DataAccess.Repositories.Products;

public class ProductControllerTests
{
    private readonly IProductRepository _productRepository = A.Fake<IProductRepository>();
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _productRepository = A.Fake<IProductRepository>();//Kolla om detta stämmer från övningsuppgifterna

        // Ställ in mock av Products-egenskapen
    }

    [Fact]
    public void GetProducts_ReturnsOkResult_WithAListOfProducts()
    {
        // Arrange

        var product = A.Dummy<Product>();

        _productRepository.Add(product);

        A.CallTo(() => _productRepository.GetAll());
        // Act
        var prodList = _productRepository.GetAll();

        // Assert
        Assert.NotNull(prodList);
        A.CallTo(() => _productRepository.GetAll()).MustHaveHappened();
    }

    [Fact]
    public void AddProduct_ReturnsOkResult()
    {
        // Arrange
        var product = A.Dummy<Product>();

        // Act
        _productRepository.Add(product);
        // Assert
        A.CallTo(() => _productRepository.Add(product)).MustHaveHappened();
    }
}
