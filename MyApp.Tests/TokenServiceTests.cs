using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Options;
using MyApp.Configurations;
using MyApp.Services;
using MyApp.Models;
using System.Collections.Generic;

namespace MyApp.Tests;

[TestFixture]
public class TokenServiceTests
{
    private TokenService _tokenService;
    private Mock<IOptions<JwtSettings>> _mockOptions;

    [SetUp]
    public void Setup()
    {
        _mockOptions = new Mock<IOptions<JwtSettings>>();
       
        _mockOptions.Setup(opt => opt.Value).Returns(new JwtSettings
        {
            SigningKey = "ThisIsAKeyThatIsLongEnoughToMeetTheRequirementsForHmacSha5121234567891011112131415161718!", 
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            TokenLifetimeDays = 7
        });

        _tokenService = new TokenService(_mockOptions.Object);
    }

    [Test]
    public void CreateToken_Returns_ValidToken()
    {
        // Arrange
        var user = new User { UserName = "testUser", Email = "test@test.com" };
        var roles = new List<string> { "Admin", "User" };

        // Act
        var token = _tokenService.CreateToken(user, roles);

        // Assert
        Assert.IsNotNull(token);
        Assert.IsInstanceOf<string>(token);
    }
}
