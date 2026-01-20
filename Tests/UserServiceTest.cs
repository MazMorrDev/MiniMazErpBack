using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MiniMazErpBack.Tests;

public class UserServiceTests
{
    private static DbContextOptions<AppDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task LoginUser_WithValidCredentials_ReturnsUser()
    {
        // Arrange
        var options = CreateNewContextOptions();
        
        // Setup: crear usuario en DB
        using (var context = new AppDbContext(options))
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
            var user = new User
            {
                Name = "testuser",
                HashedPassword = hashedPassword
            };
            
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        // Test: usar otro contexto para el servicio
        using (var context = new AppDbContext(options))
        {
            // Necesitamos crear el logger mock
            var mockLogger = new Mock<ILogger<UserService>>();
            var service = new UserService(context, mockLogger.Object);
            
            var loginDto = new LoginUserDto
            {
                Name = "testuser",
                Password = "password123"
            };

            // Act
            var result = await service.LoginUser(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Name);
        }
    }

    [Fact]
    public async Task LoginUser_WithInvalidUsername_ReturnsNull()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using var context = new AppDbContext(options);
        var mockLogger = new Mock<ILogger<UserService>>();
        var service = new UserService(context, mockLogger.Object);

        var loginDto = new LoginUserDto
        {
            Name = "nonexistent",
            Password = "password123"
        };

        // Act
        var result = await service.LoginUser(loginDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginUser_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var options = CreateNewContextOptions();
        
        using (var context = new AppDbContext(options))
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
            var user = new User
            {
                Name = "testuser",
                HashedPassword = hashedPassword
            };
            
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var mockLogger = new Mock<ILogger<UserService>>();
            var service = new UserService(context, mockLogger.Object);
            
            var loginDto = new LoginUserDto
            {
                Name = "testuser",
                Password = "wrongpassword"
            };

            // Act
            var result = await service.LoginUser(loginDto);

            // Assert
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task LoginUser_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using var context = new AppDbContext(options);
        var mockLogger = new Mock<ILogger<UserService>>();
        var service = new UserService(context, mockLogger.Object);

        var loginDto = new LoginUserDto
        {
            Name = "",
            Password = "password123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.LoginUser(loginDto));
    }

    [Fact]
    public async Task LoginUser_WithEmptyPassword_ThrowsArgumentException()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using var context = new AppDbContext(options);
        var mockLogger = new Mock<ILogger<UserService>>();
        var service = new UserService(context, mockLogger.Object);

        var loginDto = new LoginUserDto
        {
            Name = "testuser",
            Password = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.LoginUser(loginDto));
    }
}