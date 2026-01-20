using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MiniMazErpBack.Controllers;

namespace MiniMazErpBack.Tests;

public class SellControllerTests
{
    private readonly Mock<ISellService> _mockSellService;
    private readonly SellController _controller;

    public SellControllerTests()
    {
        _mockSellService = new Mock<ISellService>();
        _controller = new SellController(_mockSellService.Object);
    }

    // Helper method to create a Sell with required properties
    private Sell CreateTestSell(int id, decimal salePrice)
    {
        return new Sell 
        { 
            MovementId = id, 
            SalePrice = salePrice,
            Movement = new Movement() // Add the required Movement property
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfSells()
    {
        // Arrange
        var sells = new List<Sell>
        {
            CreateTestSell(1, 100),
            CreateTestSell(2, 200)
        };

        _mockSellService
            .Setup(service => service.GetAllSellsAsync())
            .ReturnsAsync(sells);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSells = Assert.IsAssignableFrom<IEnumerable<Sell>>(okResult.Value);
        Assert.Equal(2, returnedSells.Count());
    }

    [Fact]
    public async Task GetAll_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockSellService
            .Setup(service => service.GetAllSellsAsync())
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
        var sell = CreateTestSell(1, 100);
        
        _mockSellService
            .Setup(service => service.GetSellByIdAsync(1))
            .ReturnsAsync(sell);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSell = Assert.IsType<Sell>(okResult.Value);
        Assert.Equal(1, returnedSell.MovementId);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        _mockSellService
            .Setup(service => service.GetSellByIdAsync(1))
            .ReturnsAsync((Sell?)null);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var sellDto = new CreateSellDto 
        { 
            SalePrice = 100,
            DiscountPercentage = 10
        };
        
        var createdSell = CreateTestSell(1, 100);

        _mockSellService
            .Setup(service => service.CreateSellAsync(sellDto))
            .ReturnsAsync(createdSell);

        // Act
        var result = await _controller.Create(sellDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
        Assert.Equal(1, ((Sell)createdAtActionResult.Value!).MovementId);
    }

    [Fact]
    public async Task Create_WithInvalidSalePrice_ReturnsBadRequest()
    {
        // Arrange
        var sellDto = new CreateSellDto 
        { 
            SalePrice = 0, // Invalid
            DiscountPercentage = 10
        };

        // Act
        var result = await _controller.Create(sellDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_WithInvalidDiscount_ReturnsBadRequest()
    {
        // Arrange
        var sellDto = new CreateSellDto 
        { 
            SalePrice = 100,
            DiscountPercentage = 150 // Invalid (> 100)
        };

        // Act
        var result = await _controller.Create(sellDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var sellDto = new CreateSellDto 
        { 
            SalePrice = 100,
            DiscountPercentage = 10
        };
        
        _mockSellService
            .Setup(service => service.CreateSellAsync(sellDto))
            .ThrowsAsync(new ArgumentException("Invalid data"));

        // Act
        var result = await _controller.Create(sellDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var existingSell = CreateTestSell(1, 100);
        var updateDto = new UpdateSellDto();

        _mockSellService
            .Setup(service => service.GetSellByIdAsync(1))
            .ReturnsAsync(existingSell);

        _mockSellService
            .Setup(service => service.UpdateSellAsync(1, updateDto))
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
        var updateDto = new UpdateSellDto();

        _mockSellService
            .Setup(service => service.GetSellByIdAsync(1))
            .ReturnsAsync((Sell?)null);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ServiceReturnsFalse_ReturnsInternalServerError()
    {
        // Arrange
        var existingSell = CreateTestSell(1, 100);
        var updateDto = new UpdateSellDto();

        _mockSellService
            .Setup(service => service.GetSellByIdAsync(1))
            .ReturnsAsync(existingSell);

        _mockSellService
            .Setup(service => service.UpdateSellAsync(1, updateDto))
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
        var existingSell = CreateTestSell(1, 100);

        _mockSellService
            .Setup(service => service.GetSellByIdAsync(1))
            .ReturnsAsync(existingSell);

        _mockSellService
            .Setup(service => service.DeleteSellAsync(1))
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
        _mockSellService
            .Setup(service => service.GetSellByIdAsync(1))
            .ReturnsAsync((Sell?)null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ServiceReturnsFalse_ReturnsInternalServerError()
    {
        // Arrange
        var existingSell = CreateTestSell(1, 100);

        _mockSellService
            .Setup(service => service.GetSellByIdAsync(1))
            .ReturnsAsync(existingSell);

        _mockSellService
            .Setup(service => service.DeleteSellAsync(1))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetByProductId_ReturnsOkResult()
    {
        // Arrange
        var sells = new List<Sell>
        {
            CreateTestSell(1, 100),
            CreateTestSell(2, 200)
        };

        _mockSellService
            .Setup(service => service.GetSellsByProductIdAsync(1))
            .ReturnsAsync(sells);

        // Act
        var result = await _controller.GetByProductId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSells = Assert.IsAssignableFrom<IEnumerable<Sell>>(okResult.Value);
        Assert.Equal(2, returnedSells.Count());
    }

    [Fact]
    public async Task GetByDateRange_WithValidDates_ReturnsOkResult()
    {
        // Arrange
        var startDate = DateTimeOffset.Now.AddDays(-7);
        var endDate = DateTimeOffset.Now;
        var sells = new List<Sell>
        {
            CreateTestSell(1, 100)
        };

        _mockSellService
            .Setup(service => service.GetSellsByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(sells);

        // Act
        var result = await _controller.GetByDateRange(startDate, endDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSells = Assert.IsAssignableFrom<IEnumerable<Sell>>(okResult.Value);
        Assert.Single(returnedSells);
    }

    [Fact]
    public async Task GetByDateRange_WithInvalidDates_ReturnsBadRequest()
    {
        // Arrange
        var startDate = DateTimeOffset.Now;
        var endDate = DateTimeOffset.Now.AddDays(-7); // startDate > endDate

        // Act
        var result = await _controller.GetByDateRange(startDate, endDate);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetFullById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var fullSell = CreateTestSell(1, 100);
        
        _mockSellService
            .Setup(service => service.GetFullSellByIdAsync(1))
            .ReturnsAsync(fullSell);

        // Act
        var result = await _controller.GetFullById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSell = Assert.IsType<Sell>(okResult.Value);
        Assert.Equal(1, returnedSell.MovementId);
    }

    [Fact]
    public async Task GetFullById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        _mockSellService
            .Setup(service => service.GetFullSellByIdAsync(1))
            .ReturnsAsync((Sell?)null);

        // Act
        var result = await _controller.GetFullById(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}