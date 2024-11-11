using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApp.Controllers;
using MyApp.Dto.Create;
using MyApp.Services;
using MyApp.Models;
using MyApp.IServices;
using AutoMapper;
using MyApp.Repository.UnitOfWorks;
using DocumentFormat.OpenXml.Office2010.Excel;
using MyApp.Dto.Read;

namespace MyApp.Tests;

public class CartControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICartService> _mockCartService;
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<CartController> _controller;

    public CartControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCartService = new Mock<ICartService>();
        _mockProductService = new Mock<IProductService>();
        _mockUserService = new Mock<IUserService>();
        _mockMapper = new Mock<IMapper>();

        _controller = new Mock<CartController>(
            _mockUnitOfWork.Object,
            _mockCartService.Object,
            _mockProductService.Object,
            _mockUserService.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task AddItemToCart_ReturnsOk_WhenValidData()
    {
        //Arrange
       var cartItemDto = new CartCreateDto { ProductId = 1, Quantity = 1, UserId = 1 };
        var cartItem = new Cart
        {
            Id = 1,
            Product = new Product { Id = 1 },
            Quantity = 1,
            User = new User { Id = 1 }
        };

        _mockMapper.Setup(m => m.Map<Cart>(cartItemDto)).Returns(cartItem);
        _mockCartService.Setup(s => s.AddAsync(cartItem, CancellationToken.None)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveAsync(CancellationToken.None)).ReturnsAsync(true);

        //Act
       var result = await _controller.Object.AddToCartAsync(cartItemDto, CancellationToken.None);

       // Assert
       var createdResult = Xunit.Assert.IsType<ActionResult<CartReadDto>>(result);
    }

    //[Fact]
    //public async Task AddItemToCart_CallsServiceMethodOnce()
    //{
    //    //Arrange
    //   var cartCreateDto = new CartCreateDto
    //   {
    //       ProductId = 1,
    //       Quantity = 1,
    //       UserId = 1
    //   };
    //    var cartItem = new Cart
    //    {
    //        Product = new Product { Id = 1 },
    //        Quantity = 1,
    //        User = new User { Id = 1 }
    //    };

    //    _mockCartService.Setup(s => s.AddAsync(cartItem, CancellationToken.None)).Returns(Task.CompletedTask);

    //    //Act
    //   await _controller.AddToCartAsync(cartCreateDto, CancellationToken.None);

    //    //Assert
    //    _mockCartService.Verify(s => s.AddAsync(cartItem, CancellationToken.None), Times.Once);
    //    _mockCartService.Verify(s => s.AddAsync(It.Is<Cart>(c =>
    //    c.Product.Id == cartItem.Product.Id &&
    //    c.Quantity == cartItem.Quantity &&
    //    c.User.Id == cartItem.User.Id
    //), CancellationToken.None), Times.Once);
    //}

    //[Fact]
    //public async Task GetCartByUserId_ReturnsCart_WhenUserIdExists()
    //{
    //    // Arrange
    //    var userId = 1;
    //    var cartResponse = new CartCreateDto
    //    {
    //        UserId = userId,
    //        ProductId = 2,
    //        Quantity = 2,
    //    };

    //    _mockCartService
    //        .Setup(s => s.GetByUserId(userId))
    //        .ReturnsAsync(cartResponse);

    //    // Act
    //    var result = await _cartController.GetCartByUserId(userId);

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result);
    //    var actualResponse = Assert.IsType<CartResponseDto>(okResult.Value);
    //    Assert.Equal(cartResponse.CartId, actualResponse.CartId);
    //    Assert.Equal(cartResponse.Status, actualResponse.Status);
    //}

    //[Fact]
    //public async Task GetCartByUserId_ReturnsNotFound_WhenUserIdDoesNotExist()
    //{
    //    // Arrange
    //    var userId = 99;

    //    _cartServiceMock
    //        .Setup(s => s.GetCartByUserIdAsync(userId))
    //        .ReturnsAsync((CartResponseDto)null);

    //    // Act
    //    var result = await _cartController.GetCartByUserId(userId);

    //    // Assert
    //    Assert.IsType<NotFoundResult>(result);
    //}
}
