using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MiniMazErpBack.Tests;

public class ProductControllerTest
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly ProductController _controller;

    public ProductControllerTest()
    {
        _mockProductService = new Mock<IProductService>();
        _controller = new ProductController(_mockProductService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1" },
            new Product { Id = 2, Name = "Product 2" }
        };

        _mockProductService
            .Setup(service => service.GetAllProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Equal(2, returnedProducts.Count());
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product" };
        
        _mockProductService
            .Setup(service => service.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(1, returnedProduct.Id);
        Assert.Equal("Test Product", returnedProduct.Name);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var productDto = new CreateProductDto { Name = "New Product" };
        var createdProduct = new Product { Id = 1, Name = "New Product" };

        _mockProductService
            .Setup(service => service.CreateProductAsync(productDto))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.Create(productDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
        var returnedProduct = Assert.IsType<Product>(createdAtActionResult.Value);
        Assert.Equal(1, returnedProduct.Id);
        Assert.Equal("New Product", returnedProduct.Name);
    }

    [Fact]
    public async Task Create_ThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var productDto = new CreateProductDto { Name = "Invalid Product" };
        
        _mockProductService
            .Setup(service => service.CreateProductAsync(productDto))
            .ThrowsAsync(new ArgumentException("Invalid data"));

        // Act
        var result = await _controller.Create(productDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var existingProduct = new Product { Id = 1, Name = "Old Product" };
        var updateDto = new UpdateProductDto { Name = "Updated Product" };

        _mockProductService
            .Setup(service => service.GetProductByIdAsync(1))
            .ReturnsAsync(existingProduct);

        _mockProductService
            .Setup(service => service.UpdateProductAsync(1, updateDto))
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
        var updateDto = new UpdateProductDto { Name = "Updated Product" };

        _mockProductService
            .Setup(service => service.GetProductByIdAsync(1))
            .ReturnsAsync((Product?)null);

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
        var existingProduct = new Product { Id = 1 };
        var updateDto = new UpdateProductDto { Name = "Updated Product" };

        _mockProductService
            .Setup(service => service.GetProductByIdAsync(1))
            .ReturnsAsync(existingProduct);

        _mockProductService
            .Setup(service => service.UpdateProductAsync(1, updateDto))
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
        var existingProduct = new Product { Id = 1 };

        _mockProductService
            .Setup(service => service.GetProductByIdAsync(1))
            .ReturnsAsync(existingProduct);

        _mockProductService
            .Setup(service => service.DeleteProductAsync(1))
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
        _mockProductService
            .Setup(service => service.GetProductByIdAsync(1))
            .ReturnsAsync((Product?)null);

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
        var existingProduct = new Product { Id = 1 };

        _mockProductService
            .Setup(service => service.GetProductByIdAsync(1))
            .ReturnsAsync(existingProduct);

        _mockProductService
            .Setup(service => service.DeleteProductAsync(1))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}