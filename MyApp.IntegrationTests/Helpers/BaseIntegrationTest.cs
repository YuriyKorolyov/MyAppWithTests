using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Data;
using MyApp.Dto.Account;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace MyApp.IntegrationTests.Helpers;

public abstract class BaseIntegrationTest : IClassFixture<CustomAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly ApplicationDbContext DbContext;
    protected readonly HttpClient Client;
    private string _token;

    protected BaseIntegrationTest(CustomAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        DbContext.Database.Migrate();

        SeedDatabaseAsync().Wait();
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Метод для получения токена аутентификации.
    /// </summary>
    protected async Task<string> AuthenticateAsync()
    {
        if (_token != null) // Если токен уже есть, возвращаем его
            return _token;

        // Регистрируем тестового пользователя
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

        var registerResponse = await Client.PostAsJsonAsync("/api/account/register", registerDto);
        registerResponse.EnsureSuccessStatusCode();

        // Логинимся, чтобы получить токен
        var loginDto = new { UserName = "testUser", Password = "Test@123456789" };
        var loginResponse = await Client.PostAsJsonAsync("/api/account/login", loginDto);
        
        loginResponse.EnsureSuccessStatusCode();
        var responseString = await loginResponse.Content.ReadAsStringAsync();
        var jwtResponse = JsonConvert.DeserializeObject<NewUserDto>(responseString);

        _token = jwtResponse?.Token; // Сохраняем токен для последующего использования
        return _token;
    }

    /// <summary>
    /// Устанавливает токен в заголовок запроса для авторизации.
    /// </summary>
    protected async Task AuthenticateRequestAsync()
    {
        var token = await AuthenticateAsync();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected virtual Task SeedDatabaseAsync()
    {
        return Task.CompletedTask; 
    }

    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }
}

