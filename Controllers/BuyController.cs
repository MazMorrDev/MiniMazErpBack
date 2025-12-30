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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Buy>> GetById(int id)
    {
        try
        {
            var buy = await _buyService.GetBuyByIdAsync(id);
            if (buy == null) return NotFound($"Compra con ID {id} no encontrada");
            
            return Ok(buy);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Buy>> Create([FromBody] CreateBuyDto buyDto)
    {
        try
        {
            if (buyDto == null) return BadRequest("Buy data is invalid");
            if (buyDto.UnitPrice <= 0) return BadRequest("Unitary price must be more than 0");
            

            var createdBuy = await _buyService.CreateBuyAsync(buyDto);
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

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateBuyDto buyDto)
    {
        try
        {   
            var existingBuy = await _buyService.GetBuyByIdAsync(id);
            if (existingBuy == null) return NotFound($"Compra con ID {id} no encontrada");
        
            var result = await _buyService.UpdateBuyAsync(id, buyDto);
            if (!result) return StatusCode(500, "Error al actualizar la compra");

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
            var existingBuy = await _buyService.GetBuyByIdAsync(id);
            if (existingBuy == null) return NotFound($"Compra con ID {id} no encontrada");
            
            var result = await _buyService.DeleteBuyAsync(id);
            if (!result) return StatusCode(500, "Error al eliminar la compra");
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}