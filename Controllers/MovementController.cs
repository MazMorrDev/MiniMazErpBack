using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovementController(MovementService movementService) : ControllerBase
{
    private readonly MovementService _movementService = movementService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movement>>> GetAll()
    {
        try
        {
            var movements = await _movementService.GetAllMovementsAsync();
            return Ok(movements);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Movement>> GetById(int id)
    {
        try
        {
            var movement = await _movementService.GetMovementByIdAsync(id);
            if (movement == null)
            {
                return NotFound($"Movimiento con ID {id} no encontrado");
            }
            return Ok(movement);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Movement>> Create([FromBody] Movement movement)
    {
        try
        {
            if (movement == null)
            {
                return BadRequest("Los datos de movimiento son inválidos");
            }

            // Validar datos básicos
            if (movement.Quantity == 0)
            {
                return BadRequest("La cantidad no puede ser 0");
            }

            if (movement.WarehouseId <= 0)
            {
                return BadRequest("ID de almacén inválido");
            }

            if (movement.ProductId <= 0)
            {
                return BadRequest("ID de producto inválido");
            }

            var createdMovement = await _movementService.CreateMovementAsync(movement);
            return CreatedAtAction(nameof(GetById), new { id = createdMovement.Id }, createdMovement);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] Movement movement)
    {
        try
        {
            if (id != movement.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del objeto");
            }

            var existingMovement = await _movementService.GetMovementByIdAsync(id);
            if (existingMovement == null)
            {
                return NotFound($"Movimiento con ID {id} no encontrado");
            }

            // Verificar si tiene registros hijos
            var hasRelatedRecords = await _movementService.HasRelatedRecordsAsync(id);
            if (hasRelatedRecords)
            {
                return BadRequest("No se puede actualizar un movimiento que tiene registros relacionados (Buy/Sell/Expense)");
            }

            var result = await _movementService.UpdateMovementAsync(movement);
            if (!result)
            {
                return StatusCode(500, "Error al actualizar el movimiento");
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
            var existingMovement = await _movementService.GetMovementByIdAsync(id);
            if (existingMovement == null)
            {
                return NotFound($"Movimiento con ID {id} no encontrado");
            }

            // Verificar si tiene registros hijos
            var hasRelatedRecords = await _movementService.HasRelatedRecordsAsync(id);
            if (hasRelatedRecords)
            {
                return BadRequest("No se puede eliminar un movimiento que tiene registros relacionados (Buy/Sell/Expense)");
            }

            var result = await _movementService.DeleteMovementAsync(id);
            if (!result)
            {
                return StatusCode(500, "Error al eliminar el movimiento");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint para verificar si un movimiento tiene registros relacionados
    [HttpGet("{id}/has-related-records")]
    public async Task<ActionResult<bool>> HasRelatedRecords(int id)
    {
        try
        {
            var hasRecords = await _movementService.HasRelatedRecordsAsync(id);
            return Ok(hasRecords);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint para obtener movimientos por producto
    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<Movement>>> GetByProductId(int productId)
    {
        try
        {
            var movements = await _movementService.GetByProductIdAsync(productId);
            return Ok(movements);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint para obtener movimientos por rango de fechas
    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<Movement>>> GetByDateRange(
        [FromQuery] DateTimeOffset startDate,
        [FromQuery] DateTimeOffset endDate)
    {
        try
        {
            if (startDate > endDate)
            {
                return BadRequest("La fecha de inicio no puede ser mayor a la fecha de fin");
            }

            var movements = await _movementService.GetByDateRangeAsync(startDate, endDate);
            return Ok(movements);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}