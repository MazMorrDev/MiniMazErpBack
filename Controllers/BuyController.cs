using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuyController(BuyService buyService) : ControllerBase
{
    private readonly BuyService _buyService = buyService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Buy>>> GetAll()
    {
        try
        {
            var buys = await _buyService.GetAllBuysAsync();
            return Ok(buys);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Buy>> GetById(int id)
    {
        try
        {
            var buy = await _buyService.GetBuyByIdAsync(id);
            if (buy == null)
            {
                return NotFound($"Compra con ID {id} no encontrada");
            }
            return Ok(buy);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Buy>> Create([FromBody] Buy buy)
    {
        try
        {
            if (buy == null || buy.Movement == null)
            {
                return BadRequest("Los datos de compra son inválidos");
            }

            // Validar datos básicos
            if (buy.Movement.Quantity <= 0)
            {
                return BadRequest("La cantidad debe ser mayor a 0");
            }

            if (buy.UnitPrice <= 0)
            {
                return BadRequest("El precio unitario debe ser mayor a 0");
            }

            var createdBuy = await _buyService.CreateBuyAsync(buy);
            return CreatedAtAction(nameof(GetById), new { id = createdBuy.MovementId }, createdBuy);
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
    public async Task<ActionResult> Update(int id, [FromBody] Buy buy)
    {
        try
        {
            if (id != buy.MovementId)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del objeto");
            }

            var existingBuy = await _buyService.GetBuyByIdAsync(id);
            if (existingBuy == null)
            {
                return NotFound($"Compra con ID {id} no encontrada");
            }

            var result = await _buyService.UpdateBuyAsync(buy);
            if (!result)
            {
                return StatusCode(500, "Error al actualizar la compra");
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
            var existingBuy = await _buyService.GetBuyByIdAsync(id);
            if (existingBuy == null)
            {
                return NotFound($"Compra con ID {id} no encontrada");
            }

            var result = await _buyService.DeleteBuyAsync(id);
            if (!result)
            {
                return StatusCode(500, "Error al eliminar la compra");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint adicional para obtener compra completa con Movement
    [HttpGet("{id}/full")]
    public async Task<ActionResult<Buy>> GetFullById(int id)
    {
        try
        {
            var buy = await _buyService.GetFullBuyByIdAsync(id);
            if (buy == null)
            {
                return NotFound($"Compra con ID {id} no encontrada");
            }
            return Ok(buy);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}