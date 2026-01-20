using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MiniMazErpBack.Tests;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ILogger<UserController>> _mockLogger;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockJwtService = new Mock<IJwtService>();
        _mockLogger = new Mock<ILogger<UserController>>();

        _controller = new UserController(
            _mockUserService.Object,
            _mockJwtService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task LoginUser_WithValidCredentials_ReturnsTokenAndUser()
    {
        // Arrange
        var loginDto = new LoginUserDto
        {
            Name = "testuser",
            Password = "password123"
        };

        var user = new User
        {
            Id = 1,
            Name = "testuser",
            HashedPassword = "hashed_password"
        };

        var expectedToken = "fake-jwt-token";

        _mockUserService
            .Setup(service => service.LoginUser(loginDto))
            .ReturnsAsync(user);

        _mockJwtService
            .Setup(service => service.GenerateJwtToken(user))
            .Returns(expectedToken);

        // Act
        var result = await _controller.LoginUser(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        
        // Usar Assert.Equal en lugar de dynamic (más seguro)
        var response = okResult.Value as dynamic;
        Assert.NotNull(response);
        
        // Para acceder a las propiedades, necesitamos reflexión o convertir a tipo específico
        // Vamos a hacerlo más simple:
        var responseDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(
            Newtonsoft.Json.JsonConvert.SerializeObject(response));
        
        Assert.Equal("Login successful", responseDict["message"]);
        Assert.Equal(expectedToken, responseDict["token"]);
        
        var userDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(
            responseDict["user"].ToString());
        Assert.Equal("1", userDict["Id"].ToString()); // Los números se serializan como string en JSON
        Assert.Equal("testuser", userDict["Name"]);
    }

    [Fact]
    public async Task LoginUser_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginUserDto
        {
            Name = "wronguser",
            Password = "wrongpassword"
        };

        _mockUserService
            .Setup(service => service.LoginUser(loginDto))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.LoginUser(loginDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = unauthorizedResult.Value as dynamic;
        
        // Simplemente verificar que el resultado es Unauthorized
        Assert.NotNull(response);
    }

    [Fact]
    public async Task LoginUser_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginUserDto
        {
            Name = "", // Invalid - empty name
            Password = "password123"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.LoginUser(loginDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginUser_ThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginUserDto
        {
            Name = "testuser",
            Password = "password123"
        };

        _mockUserService
            .Setup(service => service.LoginUser(loginDto))
            .ThrowsAsync(new ArgumentException("Invalid input"));

        // Act
        var result = await _controller.LoginUser(loginDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badRequestResult.Value as dynamic;
        Assert.NotNull(response);
    }

    [Fact]
    public async Task LoginUser_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var loginDto = new LoginUserDto
        {
            Name = "testuser",
            Password = "password123"
        };

        _mockUserService
            .Setup(service => service.LoginUser(loginDto))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.LoginUser(loginDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}