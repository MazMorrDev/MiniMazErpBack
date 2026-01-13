// ClientServiceTests.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MiniMazErpBack.Tests;

public class ClientServiceTests
{
    private static DbContextOptions<AppDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task LoginClient_WithValidCredentials_ReturnsClient()
    {
        // Arrange
        var options = CreateNewContextOptions();
        
        // Setup: crear cliente en DB
        using (var context = new AppDbContext(options))
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
            var client = new Client
            {
                Name = "testuser",
                HashedPassword = hashedPassword
            };
            
            await context.Clients.AddAsync(client);
            await context.SaveChangesAsync();
        }

        // Test: usar otro contexto para el servicio
        using (var context = new AppDbContext(options))
        {
            // Necesitamos crear el logger mock
            var mockLogger = new Mock<ILogger<ClientService>>();
            var service = new ClientService(context, mockLogger.Object);
            
            var loginDto = new LoginClientDto
            {
                Name = "testuser",
                Password = "password123"
            };

            // Act
            var result = await service.LoginClient(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Name);
        }
    }

    [Fact]
    public async Task LoginClient_WithInvalidUsername_ReturnsNull()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using var context = new AppDbContext(options);
        var mockLogger = new Mock<ILogger<ClientService>>();
        var service = new ClientService(context, mockLogger.Object);

        var loginDto = new LoginClientDto
        {
            Name = "nonexistent",
            Password = "password123"
        };

        // Act
        var result = await service.LoginClient(loginDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginClient_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var options = CreateNewContextOptions();
        
        using (var context = new AppDbContext(options))
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
            var client = new Client
            {
                Name = "testuser",
                HashedPassword = hashedPassword
            };
            
            await context.Clients.AddAsync(client);
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var mockLogger = new Mock<ILogger<ClientService>>();
            var service = new ClientService(context, mockLogger.Object);
            
            var loginDto = new LoginClientDto
            {
                Name = "testuser",
                Password = "wrongpassword"
            };

            // Act
            var result = await service.LoginClient(loginDto);

            // Assert
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task LoginClient_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using var context = new AppDbContext(options);
        var mockLogger = new Mock<ILogger<ClientService>>();
        var service = new ClientService(context, mockLogger.Object);

        var loginDto = new LoginClientDto
        {
            Name = "",
            Password = "password123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.LoginClient(loginDto));
    }

    [Fact]
    public async Task LoginClient_WithEmptyPassword_ThrowsArgumentException()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using var context = new AppDbContext(options);
        var mockLogger = new Mock<ILogger<ClientService>>();
        var service = new ClientService(context, mockLogger.Object);

        var loginDto = new LoginClientDto
        {
            Name = "testuser",
            Password = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.LoginClient(loginDto));
    }
}