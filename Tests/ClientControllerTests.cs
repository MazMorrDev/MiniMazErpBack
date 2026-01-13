// ClientControllerTests.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MiniMazErpBack.Tests;

public class ClientControllerTests
{
    private readonly Mock<IClientService> _mockClientService;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ILogger<ClientController>> _mockLogger;
    private readonly ClientController _controller;

    public ClientControllerTests()
    {
        _mockClientService = new Mock<IClientService>();
        _mockJwtService = new Mock<IJwtService>();
        _mockLogger = new Mock<ILogger<ClientController>>();

        _controller = new ClientController(
            _mockClientService.Object,
            _mockJwtService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task LoginClient_WithValidCredentials_ReturnsTokenAndUser()
    {
        // Arrange
        var loginDto = new LoginClientDto
        {
            Name = "testuser",
            Password = "password123"
        };

        var client = new Client
        {
            Id = 1,
            Name = "testuser",
            HashedPassword = "hashed_password"
        };

        var expectedToken = "fake-jwt-token";

        _mockClientService
            .Setup(service => service.LoginClient(loginDto))
            .ReturnsAsync(client);

        _mockJwtService
            .Setup(service => service.GenerateJwtToken(client))
            .Returns(expectedToken);

        // Act
        var result = await _controller.LoginClient(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value as dynamic;

        Assert.Equal("Login successful", response?.message);
        Assert.Equal(expectedToken, response?.token);
        Assert.Equal(1, response?.user?.Id);
        Assert.Equal("testuser", response?.user?.Name);
    }

    [Fact]
    public async Task LoginClient_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginClientDto
        {
            Name = "wronguser",
            Password = "wrongpassword"
        };

        _mockClientService
            .Setup(service => service.LoginClient(loginDto))
            .ReturnsAsync((Client?)null);

        // Act
        var result = await _controller.LoginClient(loginDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = unauthorizedResult.Value as dynamic;
        Assert.Equal("Invalid credentials", response?.message);
    }

    [Fact]
    public async Task LoginClient_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginClientDto
        {
            Name = "", // Invalid - empty name
            Password = "password123"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.LoginClient(loginDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginClient_ThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginClientDto
        {
            Name = "testuser",
            Password = "password123"
        };

        _mockClientService
            .Setup(service => service.LoginClient(loginDto))
            .ThrowsAsync(new ArgumentException("Invalid input"));

        // Act
        var result = await _controller.LoginClient(loginDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badRequestResult.Value as dynamic;
        Assert.Equal("Invalid input", response?.error);
    }

    [Fact]
    public async Task LoginClient_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var loginDto = new LoginClientDto
        {
            Name = "testuser",
            Password = "password123"
        };

        _mockClientService
            .Setup(service => service.LoginClient(loginDto))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.LoginClient(loginDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);

        var response = statusCodeResult.Value as dynamic;
        Assert.Equal("Internal server error", response?.error);
    }
}