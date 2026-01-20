using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SellController(ISellService sellService) : ControllerBase
{
    private readonly ISellService _sellService = sellService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sell>>> GetAll()
    {
        try
        {
            var sells = await _sellService.GetAllSellsAsync();
            return Ok(sells);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Sell>> GetById(int id)
    {
        try
        {
            var sell = await _sellService.GetSellByIdAsync(id);
            if (sell == null) return NotFound($"Venta con ID {id} no encontrada");
            
            return Ok(sell);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Sell>> Create([FromBody] CreateSellDto sellDto)
    {
        try
        {
            // Validar datos básicos
            if (sellDto.SalePrice <= 0) return BadRequest("El precio de venta debe ser mayor a 0");
            
            if (sellDto.DiscountPercentage < 0 || sellDto.DiscountPercentage > 100) 
                return BadRequest("El porcentaje de descuento debe estar entre 0 y 100");

            var createdSell = await _sellService.CreateSellAsync(sellDto);
            return CreatedAtAction(nameof(GetById), new { id = createdSell.MovementId }, createdSell);
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
    public async Task<ActionResult> Update(int id, [FromBody] UpdateSellDto sellDto)
    {
        try
        {
            var existingSell = await _sellService.GetSellByIdAsync(id);
            if (existingSell == null) return NotFound($"Venta con ID {id} no encontrada");
            
            var result = await _sellService.UpdateSellAsync(id, sellDto);
            if (!result) return StatusCode(500, "Error al actualizar la venta");
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var existingSell = await _sellService.GetSellByIdAsync(id);
            if (existingSell == null) return NotFound($"Venta con ID {id} no encontrada");
            

            var result = await _sellService.DeleteSellAsync(id);
            if (!result) return StatusCode(500, "Error al eliminar la venta");
            

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint para obtener ventas por producto
    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<Sell>>> GetByProductId(int productId)
    {
        try
        {
            var sells = await _sellService.GetSellsByProductIdAsync(productId);
            return Ok(sells);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint para obtener ventas por rango de fechas
    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<Sell>>> GetByDateRange(
        [FromQuery] DateTimeOffset startDate,
        [FromQuery] DateTimeOffset endDate)
    {
        try
        {
            if (startDate > endDate) return BadRequest("La fecha de inicio no puede ser mayor a la fecha de fin");

            var sells = await _sellService.GetSellsByDateRangeAsync(startDate, endDate);
            return Ok(sells);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint para obtener venta completa
    [HttpGet("{id:int}/full")]
    public async Task<ActionResult<Sell>> GetFullById(int id)
    {
        try
        {
            var sell = await _sellService.GetFullSellByIdAsync(id);
            if (sell == null) return NotFound($"Venta con ID {id} no encontrada");
            
            return Ok(sell);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}