using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MiniMazErpBack.Tests;

public class InventoryControllerTests
{
    private readonly Mock<IInventoryService> _mockInventoryService;
    private readonly InventoryController _controller;

    public InventoryControllerTests()
    {
        _mockInventoryService = new Mock<IInventoryService>();
        _controller = new InventoryController(_mockInventoryService.Object);
    }

    // Helper method to create a CreateInventoryDto with required properties
    private static CreateInventoryDto CreateTestCreateInventoryDto()
    {
        return new CreateInventoryDto
        {
            UserId = 1,
            ProductId = 1,
            Stock = 100
            // Add any other required properties
        };
    }

    // Helper method to create an UpdateInventoryDto with required properties
    private static UpdateInventoryDto CreateTestUpdateInventoryDto()
    {
        return new UpdateInventoryDto
        {
            UserId = 1,
            ProductId = 1,
            Stock = 150
            // Add any other required properties
        };
    }

    // Helper method to create an Inventory with required properties (if needed)
    private Inventory CreateTestInventory(int id)
    {
        return new Inventory 
        { 
            Id = id,
            // Add any required properties if Inventory has them
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfInventories()
    {
        // Arrange
        var inventories = new List<Inventory>
        {
            CreateTestInventory(1),
            CreateTestInventory(2)
        };

        _mockInventoryService
            .Setup(service => service.GetAllInventoriesAsync())
            .ReturnsAsync(inventories);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedInventories = Assert.IsAssignableFrom<IEnumerable<Inventory>>(okResult.Value);
        Assert.Equal(2, returnedInventories.Count());
    }

    [Fact]
    public async Task GetAll_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockInventoryService
            .Setup(service => service.GetAllInventoriesAsync())
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var inventory = CreateTestInventory(1);
        
        _mockInventoryService
            .Setup(service => service.GetInventoryByIdAsync(1))
            .ReturnsAsync(inventory);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedInventory = Assert.IsType<Inventory>(okResult.Value);
        Assert.Equal(1, returnedInventory.Id);
    }

    [Fact]
    public async Task GetById_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockInventoryService
            .Setup(service => service.GetInventoryByIdAsync(1))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var inventoryDto = CreateTestCreateInventoryDto();
        var createdInventory = CreateTestInventory(1);

        _mockInventoryService
            .Setup(service => service.CreateInventoryAsync(inventoryDto))
            .ReturnsAsync(createdInventory);

        // Act
        var result = await _controller.Create(inventoryDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
        Assert.Equal(1, ((Inventory)createdAtActionResult.Value!).Id);
    }

    [Fact]
    public async Task Create_ThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var inventoryDto = CreateTestCreateInventoryDto();
        
        _mockInventoryService
            .Setup(service => service.CreateInventoryAsync(inventoryDto))
            .ThrowsAsync(new ArgumentException("Invalid data"));

        // Act
        var result = await _controller.Create(inventoryDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var inventoryDto = CreateTestCreateInventoryDto();
        
        _mockInventoryService
            .Setup(service => service.CreateInventoryAsync(inventoryDto))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Create(inventoryDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var existingInventory = CreateTestInventory(1);
        var updateDto = CreateTestUpdateInventoryDto();

        _mockInventoryService
            .Setup(service => service.GetInventoryByIdAsync(1))
            .ReturnsAsync(existingInventory);

        _mockInventoryService
            .Setup(service => service.UpdateInventoryAsync(1, updateDto))
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
        var updateDto = CreateTestUpdateInventoryDto();

        _mockInventoryService
            .Setup(service => service.GetInventoryByIdAsync(1))
            .ReturnsAsync((Inventory?)null);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("no encontrado", notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task Update_ServiceReturnsFalse_ReturnsInternalServerError()
    {
        // Arrange
        var existingInventory = CreateTestInventory(1);
        var updateDto = CreateTestUpdateInventoryDto();

        _mockInventoryService
            .Setup(service => service.GetInventoryByIdAsync(1))
            .ReturnsAsync(existingInventory);

        _mockInventoryService
            .Setup(service => service.UpdateInventoryAsync(1, updateDto))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Update_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var existingInventory = CreateTestInventory(1);
        var updateDto = CreateTestUpdateInventoryDto();

        _mockInventoryService
            .Setup(service => service.GetInventoryByIdAsync(1))
            .ReturnsAsync(existingInventory);

        _mockInventoryService
            .Setup(service => service.UpdateInventoryAsync(1, updateDto))
            .ThrowsAsync(new Exception("Database error"));

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
        var existingInventory = CreateTestInventory(1);

        _mockInventoryService
            .Setup(service => service.GetInventoryByIdAsync(1))
            .ReturnsAsync(existingInventory);

        _mockInventoryService
            .Setup(service => service.DeleteInventoryAsync(1))
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
        _mockInventoryService
            .Setup(service => service.GetInventoryByIdAsync(1))
            .ReturnsAsync((Inventory?)null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("no encontrado", notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task Delete_ServiceReturnsFalse_ReturnsInternalServerError()
    {
        // Arrange
        var existingInventory = CreateTestInventory(1);

        _mockInventoryService
            .Setup(service => service.GetInventoryByIdAsync(1))
            .ReturnsAsync(existingInventory);

        _mockInventoryService
            .Setup(service => service.DeleteInventoryAsync(1))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Delete_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var existingInventory = CreateTestInventory(1);

        _mockInventoryService
            .Setup(service => service.GetInventoryByIdAsync(1))
            .ReturnsAsync(existingInventory);

        _mockInventoryService
            .Setup(service => service.DeleteInventoryAsync(1))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}