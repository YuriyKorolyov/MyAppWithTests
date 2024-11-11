using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using MyApp.Dto.Account;
using MyApp.IntegrationTests.Helpers;
using Xunit;

namespace MyApp.IntegrationTests;

public class AccountControllerTests : BaseIntegrationTest
{
    public AccountControllerTests(CustomAppFactory factory) : base(factory) { }

    [Fact]
    public async Task RegisterUser_ReturnsOk_WhenValidData()
    {
        var registerDto = new RegisterDto
        {
            UserName = "testUser",
            Email = "test@test.com",
            Password = "Test@123456789",
            PhoneNumber = "1234567890",
            FirstName = "Test",
            LastName = "User",
            ShippingAddress = "Test Address"
        };

        var response = await Client.PostAsJsonAsync("/api/account/register", registerDto);

        // Assert
        Xunit.Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

