using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MiniMazErpBack.Tests;

public class MovementControllerTest
{
    private readonly Mock<IMovementService> _mockMovementService;
    private readonly MovementController _controller;

    public MovementControllerTest()
    {
        _mockMovementService = new Mock<IMovementService>();
        _controller = new MovementController(_mockMovementService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfMovements()
    {
        // Arrange
        var movements = new List<Movement>
        {
            new Movement { Id = 1 },
            new Movement { Id = 2 }
        };

        _mockMovementService
            .Setup(service => service.GetAllMovementsAsync())
            .ReturnsAsync(movements);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedMovements = Assert.IsAssignableFrom<IEnumerable<Movement>>(okResult.Value);
        Assert.Equal(2, returnedMovements.Count());
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var movement = new Movement { Id = 1 };
        
        _mockMovementService
            .Setup(service => service.GetMovementByIdAsync(1))
            .ReturnsAsync(movement);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedMovement = Assert.IsType<Movement>(okResult.Value);
        Assert.Equal(1, returnedMovement.Id);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        _mockMovementService
            .Setup(service => service.GetMovementByIdAsync(1))
            .ReturnsAsync((Movement?)null);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var movementDto = new CreateMovementDto();
        var createdMovement = new Movement { Id = 1 };

        _mockMovementService
            .Setup(service => service.CreateMovementAsync(movementDto))
            .ReturnsAsync(createdMovement);

        // Act
        var result = await _controller.Create(movementDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
        Assert.Equal(1, ((Movement)createdAtActionResult.Value!).Id);
    }

    [Fact]
    public async Task Create_WithNullDto_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.Create(null!);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var movementDto = new CreateMovementDto();
        
        _mockMovementService
            .Setup(service => service.CreateMovementAsync(movementDto))
            .ThrowsAsync(new ArgumentException("Invalid data"));

        // Act
        var result = await _controller.Create(movementDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var existingMovement = new Movement { Id = 1 };
        var updateDto = new UpdateMovementDto();

        _mockMovementService
            .Setup(service => service.GetMovementByIdAsync(1))
            .ReturnsAsync(existingMovement);

        _mockMovementService
            .Setup(service => service.UpdateMovementAsync(1, updateDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateMovementDto();

        _mockMovementService
            .Setup(service => service.GetMovementByIdAsync(1))
            .ReturnsAsync((Movement?)null);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ServiceReturnsFalse_ReturnsInternalServerError()
    {
        // Arrange
        var existingMovement = new Movement { Id = 1 };
        var updateDto = new UpdateMovementDto();

        _mockMovementService
            .Setup(service => service.GetMovementByIdAsync(1))
            .ReturnsAsync(existingMovement);

        _mockMovementService
            .Setup(service => service.UpdateMovementAsync(1, updateDto))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var existingMovement = new Movement { Id = 1 };

        _mockMovementService
            .Setup(service => service.GetMovementByIdAsync(1))
            .ReturnsAsync(existingMovement);

        _mockMovementService
            .Setup(service => service.DeleteMovementAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        _mockMovementService
            .Setup(service => service.GetMovementByIdAsync(1))
            .ReturnsAsync((Movement?)null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ServiceReturnsFalse_ReturnsInternalServerError()
    {
        // Arrange
        var existingMovement = new Movement { Id = 1 };

        _mockMovementService
            .Setup(service => service.GetMovementByIdAsync(1))
            .ReturnsAsync(existingMovement);

        _mockMovementService
            .Setup(service => service.DeleteMovementAsync(1))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}