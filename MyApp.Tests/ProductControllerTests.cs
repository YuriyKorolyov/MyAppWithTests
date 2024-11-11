using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.Models;
using NUnit.Framework;
using MyApp.Controllers;
using AutoMapper;
using Moq;
using MyApp.IServices;
using MyApp.Repository.UnitOfWorks;
using MyApp.Dto.Read;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;

namespace MyApp.Tests;

[TestFixture]
public class ProductControllerTests
{

    //[Test]
    //public async Task GetProductsAsync_ReturnsOkResult_WithListOfProducts()
    //{
    //    // Arrange
    //    Mock<IProductService> _mockProductService = new Mock<IProductService>();
    //    Mock<IMapper> _mockMapper = new Mock<IMapper>();
    //    ProductsController _controller = new ProductsController(
    //        Mock.Of<IUnitOfWork>(),
    //        _mockProductService.Object,
    //        Mock.Of<IReviewService>(),
    //        Mock.Of<ICategoryService>(),
    //        _mockMapper.Object);

    //    var testProducts = new List<Product>
    //        {
    //            new Product { Id = 1, Name = "Demo1", Price = 1 },
    //            new Product { Id = 2, Name = "Demo2", Price = 3.75M },
    //            new Product { Id = 3, Name = "Demo3", Price = 16.99M },
    //            new Product { Id = 4, Name = "Demo4", Price = 11.00M }
    //        };

    //    var productDtos = new List<ProductReadDto>
    //        {
    //            new ProductReadDto { Id = 1, Name = "Demo1", Price = 1 },
    //            new ProductReadDto { Id = 2, Name = "Demo2", Price = 3.75M },
    //            new ProductReadDto { Id = 3, Name = "Demo3", Price = 16.99M },
    //            new ProductReadDto { Id = 4, Name = "Demo4", Price = 11.00M }
    //        };

    //    _mockProductService.Setup(s => s.GetAll()).Returns(testProducts.AsQueryable());
    //    _mockMapper.Setup(m => m.ProjectTo<ProductReadDto>(It.IsAny<IQueryable<Product>>(), null))
    //               .Returns(productDtos.AsQueryable());

    //    // Act
    //    var result = await _controller.GetProductsAsync(CancellationToken.None);

    //    // Assert
    //    Assert.IsInstanceOf<OkObjectResult>(result.Result);
    //    var okResult = result.Result as OkObjectResult;
    //    Assert.IsInstanceOf<List<ProductReadDto>>(okResult.Value);
    //    var returnValue = okResult.Value as List<ProductReadDto>;
    //    Assert.AreEqual(4, returnValue.Count);
    //}
    [Test]
    public async Task GetProduct_ShouldReturnCorrectProduct()
    {
        // Arrange
        var testProducts = GetTestProducts();
        var mockProductService = new Mock<IProductService>();
        var mockMapper = new Mock<IMapper>();

        // Настраиваем mockProductService для проверки существования продукта
        mockProductService.Setup(service => service.ExistsAsync(4, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(true); // Продукт с ID = 4 существует

        // Настраиваем mockProductService для возврата продукта по ID
        var product = testProducts.Single(p => p.Id == 4);
        mockProductService.Setup(service => service.GetByIdAsync(4, It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(product);

        // Настраиваем mockMapper для преобразования Product в ProductReadDto
        mockMapper.Setup(mapper => mapper.Map<ProductReadDto>(It.IsAny<Product>()))
                  .Returns((Product p) => new ProductReadDto { Id = p.Id, Name = p.Name });

        var controller = new ProductsController(
            new Mock<IUnitOfWork>().Object,
            mockProductService.Object,
            new Mock<IReviewService>().Object,
            new Mock<ICategoryService>().Object,
            mockMapper.Object
        );

        // Act
        var result = await controller.GetProductByIdAsync(4, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var productDto = okResult.Value as ProductReadDto;
        Assert.IsNotNull(productDto);
        Assert.AreEqual("Demo4", productDto.Name);
    }


    [Test]
    public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var mockProductService = new Mock<IProductService>();
        mockProductService.Setup(service => service.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((Product)null);

        var controller = new ProductsController(
            new Mock<IUnitOfWork>().Object,
            mockProductService.Object,
            new Mock<IReviewService>().Object,
            new Mock<ICategoryService>().Object,
            new Mock<IMapper>().Object
        );

        // Act
        var result = await controller.GetProductByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result.Result);
    }

    private List<Product> GetTestProducts()
    {
        var testProducts = new List<Product>();
        testProducts.Add(new Product { Id = 1, Name = "Demo1", Price = 1 });
        testProducts.Add(new Product { Id = 2, Name = "Demo2", Price = 3.75M });
        testProducts.Add(new Product { Id = 3, Name = "Demo3", Price = 16.99M });
        testProducts.Add(new Product { Id = 4, Name = "Demo4", Price = 11.00M });

        return testProducts;
    }
}