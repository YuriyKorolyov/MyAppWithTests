using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using MyApp.IntegrationTests.Helpers;
using MyApp.Dto.Account;
using MyApp.Models;
using System;

namespace MyApp.IntegrationTests;

public class CartControllerTests : BaseIntegrationTest
{
    public CartControllerTests(CustomAppFactory factory) : base(factory) { }

    protected override async Task SeedDatabaseAsync()
    {
        var product1 = new Product
        {
            Name = "test product1",
            Description = "test description",
            Price = 150,
            StockQuantity = 10,
            ImageUrl = "test image url"
        };

        var product2 = new Product
        {
            Name = "test product2",
            Description = "test description",
            Price = 150,
            StockQuantity = 10,
            ImageUrl = "test image url"
        };

        await DbContext.Products.AddAsync(product1);
        await DbContext.Products.AddAsync(product2);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task PlaceOrder_ReturnsOk_WhenValidData()
    {
        // Аутентифицируем пользователя и устанавливаем токен
        await AuthenticateRequestAsync();

        // Act: Add product to cart
        var cartDto = new { ProductId = 1, Quantity = 2, UserId = 1 };
        var response = await Client.PostAsJsonAsync("/api/cart", cartDto);

        // Assert: Ensure cart was placed successfully
        Xunit.Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}

