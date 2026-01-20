using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MovementController(IMovementService movementService) : ControllerBase
{
    private readonly IMovementService _service = movementService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movement>>> GetAll()
    {
        try
        {
            var movements = await _service.GetAllMovementsAsync();
            return Ok(movements);
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
            var movement = await _service.GetMovementByIdAsync(id);
            if (movement == null) return NotFound($"Movimiento con ID {id} no encontrado");

            return Ok(movement);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Buy>> Create([FromBody] CreateMovementDto movementDto)
    {
        try
        {
            if (movementDto == null) return BadRequest("Movement data is invalid");

            var createdMovement = await _service.CreateMovementAsync(movementDto);
            return CreatedAtAction(nameof(GetById), new { id = createdMovement.Id }, createdMovement);
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
    public async Task<ActionResult> Update(int id, [FromBody] UpdateMovementDto movementDto)
    {
        try
        {
            var existingMovement = await _service.GetMovementByIdAsync(id);
            if (existingMovement == null) return NotFound($"Movimiento con ID {id} no encontrada");

            var result = await _service.UpdateMovementAsync(id, movementDto);
            if (!result) return StatusCode(500, "Error al actualizar el movimiento");

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
            var existingMovement = await _service.GetMovementByIdAsync(id);
            if (existingMovement == null) return NotFound($"Movimiento con ID {id} no encontrada");

            var result = await _service.DeleteMovementAsync(id);
            if (!result) return StatusCode(500, "Error al eliminar el movimiento");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}
