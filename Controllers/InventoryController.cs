using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController(InventoryService service) : ControllerBase
{
    private readonly InventoryService _service = service;

    [HttpPost]
    public async Task<IActionResult> Create(CreateInventoryDto inventoryDto)
    {
        try
        {
            var createdInventory = await _service.CreateInventoryAsync(inventoryDto);
            return CreatedAtAction(nameof(GetById), new { id = createdInventory.Id }, createdInventory);
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
    public async Task<IActionResult> Update(int id, UpdateInventoryDto inventoryDto)
    {
        try
        {
            var existingInventory = await _service.GetInventoryByIdAsync(id);
            if (existingInventory == null) return NotFound($"Inventario con ID {id} no encontrado");

            var result = await _service.UpdateInventoryAsync(id, inventoryDto);
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
            var existingInventory = await _service.GetInventoryByIdAsync(id);
            if (existingInventory == null) return NotFound($"Gasto con ID {id} no encontrado");

            var result = await _service.DeleteInventoryAsync(id);
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
            return Ok(await _service.GetAllInventoriesAsync());
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
            return Ok(await _service.GetInventoryByIdAsync(id));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor {ex.Message}");
        }
    }
}
