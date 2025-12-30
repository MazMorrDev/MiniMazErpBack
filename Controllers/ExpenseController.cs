using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpenseController(ExpenseService expenseService) : ControllerBase
{
    private readonly ExpenseService _expenseService = expenseService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Expense>>> GetAll()
    {
        try
        {
            var expenses = await _expenseService.GetAllExpensesAsync();
            return Ok(expenses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Expense>> GetById(int id)
    {
        try
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound($"Gasto con ID {id} no encontrado");
            }
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Expense>> Create([FromBody] Expense expense)
    {
        try
        {
            if (expense == null || expense.Movement == null)
            {
                return BadRequest("Los datos de gasto son inválidos");
            }

            // Validar datos básicos
            if (expense.TotalPrice <= 0)
            {
                return BadRequest("El precio total debe ser mayor a 0");
            }

            if (!Enum.IsDefined(expense.ExpenseType))
            {
                return BadRequest("Tipo de gasto inválido");
            }

            var createdExpense = await _expenseService.CreateExpenseAsync(expense);
            return CreatedAtAction(nameof(GetById), new { id = createdExpense.MovementId }, createdExpense);
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
    public async Task<ActionResult> Update(int id, [FromBody] Expense expense)
    {
        try
        {
            if (id != expense.MovementId)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del objeto");
            }

            var existingExpense = await _expenseService.GetExpenseByIdAsync(id);
            if (existingExpense == null)
            {
                return NotFound($"Gasto con ID {id} no encontrado");
            }

            var result = await _expenseService.UpdateExpenseAsync(expense);
            if (!result)
            {
                return StatusCode(500, "Error al actualizar el gasto");
            }

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
            var existingExpense = await _expenseService.GetExpenseByIdAsync(id);
            if (existingExpense == null)
            {
                return NotFound($"Gasto con ID {id} no encontrado");
            }

            var result = await _expenseService.DeleteExpenseAsync(id);
            if (!result)
            {
                return StatusCode(500, "Error al eliminar el gasto");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint para eliminar completamente (Expense + Movement)
    [HttpDelete("{id}/complete")]
    public async Task<ActionResult> DeleteComplete(int id)
    {
        try
        {
            var existingExpense = await _expenseService.GetExpenseByIdAsync(id);
            if (existingExpense == null)
            {
                return NotFound($"Gasto con ID {id} no encontrado");
            }

            var result = await _expenseService.DeleteExpenseAndMovementAsync(id);
            if (!result)
            {
                return StatusCode(500, "Error al eliminar el gasto completamente");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint para obtener gasto completo con Movement
    [HttpGet("{id}/full")]
    public async Task<ActionResult<Expense>> GetFullById(int id)
    {
        try
        {
            var expense = await _expenseService.GetFullExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound($"Gasto con ID {id} no encontrado");
            }
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    // Endpoint para filtrar por tipo de gasto
    [HttpGet("type/{expenseType}")]
    public async Task<ActionResult<IEnumerable<Expense>>> GetByType(ExpenseType expenseType)
    {
        try
        {
            var expenses = await _expenseService.GetByTypeAsync(expenseType);
            return Ok(expenses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}