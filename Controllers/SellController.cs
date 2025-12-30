using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellController(SellService sellService) : ControllerBase
{
    private readonly SellService _sellService = sellService;

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

    [HttpGet("{id}")]
    public async Task<ActionResult<Sell>> GetById(int id)
    {
        try
        {
            var sell = await _sellService.GetSellByIdAsync(id);
            if (sell == null)
            {
                return NotFound($"Venta con ID {id} no encontrada");
            }
            return Ok(sell);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Sell>> Create([FromBody] Sell sell)
    {
        try
        {
            if (sell == null || sell.Movement == null)
            {
                return BadRequest("Los datos de venta son inválidos");
            }

            // Validar datos básicos
            if (sell.SalePrice <= 0)
            {
                return BadRequest("El precio de venta debe ser mayor a 0");
            }

            if (sell.DiscountPercentage < 0 || sell.DiscountPercentage > 100)
            {
                return BadRequest("El porcentaje de descuento debe estar entre 0 y 100");
            }

            if (sell.Movement.Quantity <= 0)
            {
                return BadRequest("La cantidad debe ser mayor a 0");
            }

            var createdSell = await _sellService.CreateSellAsync(sell);
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

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] Sell sell)
    {
        try
        {
            if (id != sell.MovementId)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del objeto");
            }

            var existingSell = await _sellService.GetSellByIdAsync(id);
            if (existingSell == null)
            {
                return NotFound($"Venta con ID {id} no encontrada");
            }

            var result = await _sellService.UpdateSellAsync(sell);
            if (!result)
            {
                return StatusCode(500, "Error al actualizar la venta");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var existingSell = await _sellService.GetSellByIdAsync(id);
            if (existingSell == null)
            {
                return NotFound($"Venta con ID {id} no encontrada");
            }

            var result = await _sellService.DeleteSellAsync(id);
            if (!result)
            {
                return StatusCode(500, "Error al eliminar la venta");
            }

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
            if (startDate > endDate)
            {
                return BadRequest("La fecha de inicio no puede ser mayor a la fecha de fin");
            }

            var sells = await _sellService.GetSellsByDateRangeAsync(startDate, endDate);
            return Ok(sells);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint para obtener venta completa
    [HttpGet("{id}/full")]
    public async Task<ActionResult<Sell>> GetFullById(int id)
    {
        try
        {
            var sell = await _sellService.GetFullSellByIdAsync(id);
            if (sell == null)
            {
                return NotFound($"Venta con ID {id} no encontrada");
            }
            return Ok(sell);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}