using MyApp.Dto.Read;
using MyApp.IntegrationTests.Helpers;
using MyApp.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;

namespace MyApp.IntegrationTests;

public class ProductControllerTests : BaseIntegrationTest
{
    public ProductControllerTests(CustomAppFactory factory) : base(factory) { }

    protected override async Task SeedDatabaseAsync()
    {
        var product1 = new Product
        {
            Name = "Smartphone",
            Description = "Latest model smartphone",
            Price = 499,
            StockQuantity = 100,
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
    public async Task GetProduct_ReturnsOK_WhenValidData()
    {
        // Arrange: Create product data


        // Act: Send request to get product
        var response = await Client.GetAsync("/api/products/1");

        // Assert: Ensure product was successfully created
        Xunit.Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsListOfProducts_WhenProductsExist()
    {
        // Arrange: создаем несколько тестовых продуктов

        // Act: Выполняем запрос на получение всех товаров
        var response = await Client.GetAsync("/api/products");

        // Assert: Проверяем успешный статус
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var fetchedProducts = JsonConvert.DeserializeObject<List<ProductReadDto>>(responseBody);

        // Проверяем, что возвращаемый список не null 
        Xunit.Assert.NotNull(fetchedProducts);
    }
}
