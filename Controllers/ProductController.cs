using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductService service) : ControllerBase
{
    private readonly IProductService _service = service;

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto productDto)
    {
        try
        {
            var createdProduct = await _service.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto productDto)
    {
        try
        {
            var existingProduct = await _service.GetProductByIdAsync(id);
            if (existingProduct == null) return NotFound($"Gasto con ID {id} no encontrado");

            var result = await _service.UpdateProductAsync(id, productDto);
            if (!result) return StatusCode(500, "Error al actualizar el gasto");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existingProduct = await _service.GetProductByIdAsync(id);
            if (existingProduct == null) return NotFound($"Gasto con ID {id} no encontrado");

            var result = await _service.DeleteProductAsync(id);
            if (!result) return StatusCode(500, "Error al eliminar el gasto");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _service.GetAllProductsAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor {ex.Message}");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            return Ok(await _service.GetProductByIdAsync(id));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor {ex.Message}");
        }
    }
}
