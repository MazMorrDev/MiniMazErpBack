using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MiniMazErpBack.Controllers;

namespace MiniMazErpBack.Tests;

public class BuyControllerTests
{
    private readonly Mock<IBuyService> _mockBuyService;
    private readonly BuyController _controller;

    public BuyControllerTests()
    {
        _mockBuyService = new Mock<IBuyService>();
        _controller = new BuyController(_mockBuyService.Object);
    }

    // Helper method to create a Buy with required properties
    private Buy CreateTestBuy(int id, decimal unitPrice)
    {
        return new Buy 
        { 
            MovementId = id, 
            UnitPrice = unitPrice,
            Movement = new Movement() // Add the required Movement property
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfBuys()
    {
        // Arrange
        var buys = new List<Buy>
        {
            CreateTestBuy(1, 100),
            CreateTestBuy(2, 200)
        };

        _mockBuyService
            .Setup(service => service.GetAllBuysAsync())
            .ReturnsAsync(buys);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBuys = Assert.IsAssignableFrom<IEnumerable<Buy>>(okResult.Value);
        Assert.Equal(2, returnedBuys.Count());
    }

    [Fact]
    public async Task GetAll_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockBuyService
            .Setup(service => service.GetAllBuysAsync())
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var buy = CreateTestBuy(1, 100);
        
        _mockBuyService
            .Setup(service => service.GetBuyByIdAsync(1))
            .ReturnsAsync(buy);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBuy = Assert.IsType<Buy>(okResult.Value);
        Assert.Equal(1, returnedBuy.MovementId);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        _mockBuyService
            .Setup(service => service.GetBuyByIdAsync(1))
            .ReturnsAsync((Buy?)null);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var buyDto = new CreateBuyDto { UnitPrice = 100 };
        var createdBuy = CreateTestBuy(1, 100);

        _mockBuyService
            .Setup(service => service.CreateBuyAsync(buyDto))
            .ReturnsAsync(createdBuy);

        // Act
        var result = await _controller.Create(buyDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
        Assert.Equal(1, ((Buy)createdAtActionResult.Value!).MovementId);
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
    public async Task Create_WithInvalidUnitPrice_ReturnsBadRequest()
    {
        // Arrange
        var buyDto = new CreateBuyDto { UnitPrice = 0 };

        // Act
        var result = await _controller.Create(buyDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var buyDto = new CreateBuyDto { UnitPrice = 100 };
        
        _mockBuyService
            .Setup(service => service.CreateBuyAsync(buyDto))
            .ThrowsAsync(new ArgumentException("Invalid data"));

        // Act
        var result = await _controller.Create(buyDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var existingBuy = CreateTestBuy(1, 100);
        var updateDto = new UpdateBuyDto();

        _mockBuyService
            .Setup(service => service.GetBuyByIdAsync(1))
            .ReturnsAsync(existingBuy);

        _mockBuyService
            .Setup(service => service.UpdateBuyAsync(1, updateDto))
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
        var updateDto = new UpdateBuyDto();

        _mockBuyService
            .Setup(service => service.GetBuyByIdAsync(1))
            .ReturnsAsync((Buy?)null);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ServiceReturnsFalse_ReturnsInternalServerError()
    {
        // Arrange
        var existingBuy = CreateTestBuy(1, 100);
        var updateDto = new UpdateBuyDto();

        _mockBuyService
            .Setup(service => service.GetBuyByIdAsync(1))
            .ReturnsAsync(existingBuy);

        _mockBuyService
            .Setup(service => service.UpdateBuyAsync(1, updateDto))
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
        var existingBuy = CreateTestBuy(1, 100);

        _mockBuyService
            .Setup(service => service.GetBuyByIdAsync(1))
            .ReturnsAsync(existingBuy);

        _mockBuyService
            .Setup(service => service.DeleteBuyAsync(1))
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
        _mockBuyService
            .Setup(service => service.GetBuyByIdAsync(1))
            .ReturnsAsync((Buy?)null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ServiceReturnsFalse_ReturnsInternalServerError()
    {
        // Arrange
        var existingBuy = CreateTestBuy(1, 100);

        _mockBuyService
            .Setup(service => service.GetBuyByIdAsync(1))
            .ReturnsAsync(existingBuy);

        _mockBuyService
            .Setup(service => service.DeleteBuyAsync(1))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}
