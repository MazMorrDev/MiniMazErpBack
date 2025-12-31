using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[Controller]")]
public class WarehouseController(WarehouseService warehouseService) : ControllerBase
{
    private readonly WarehouseService _service = warehouseService;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWarehouseById(int id)
    {
        try
        {
            return Ok(await _service.GetWarehouseByIdAsync(id));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error del servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseDto warehouseDto)
    {
        try
        {
            var createdWarehouse = await _service.CreateWarehouseAsync(warehouseDto);
            return CreatedAtAction(nameof(GetWarehouseById), new { id = createdWarehouse.ClientId }, createdWarehouse);
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
    public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] UpdateWarehouseDto warehouseDto)
    {
        try
        {
            var existingWarehouse = await _service.GetWarehouseByIdAsync(id);
            if (existingWarehouse == null) return NotFound($"Gasto con ID {id} no encontrado");

            var result = await _service.UpdateWarehouseAsync(id, warehouseDto);
            if (!result) return StatusCode(500, "Error al actualizar el gasto");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}
