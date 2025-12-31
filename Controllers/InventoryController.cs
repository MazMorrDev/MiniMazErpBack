using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[controller]")]
public class InventoryController(IInventoryService service) : ControllerBase
{
    private readonly IInventoryService _service = service;

    [HttpPost]
    public async Task<IActionResult> Create(CreateInventoryDto inventoryDto)
    {
        try
        {

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

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {

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

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor {ex.Message}");
        }
    }
}
