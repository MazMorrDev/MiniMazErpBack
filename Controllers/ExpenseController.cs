using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
            if (expense == null) return NotFound($"Gasto con ID {id} no encontrado");
            
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Expense>> Create([FromBody] CreateExpenseDto expenseDto)
    {
        try
        {
            // Validar datos básicos
            if (expenseDto.TotalPrice <= 0) return BadRequest("El precio total debe ser mayor a 0");
        
            if (!Enum.IsDefined(expenseDto.ExpenseType)) return BadRequest("Tipo de gasto inválido"); 

            var createdExpense = await _expenseService.CreateExpenseAsync(expenseDto);
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
    public async Task<ActionResult> Update(int id, [FromBody] UpdateExpenseDto expenseDto)
    {
        try
        {
            var existingExpense = await _expenseService.GetExpenseByIdAsync(id);
            if (existingExpense == null) return NotFound($"Gasto con ID {id} no encontrado");

            var result = await _expenseService.UpdateExpenseAsync(id, expenseDto);
            if (!result) return StatusCode(500, "Error al actualizar el gasto");

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
            if (existingExpense == null) return NotFound($"Gasto con ID {id} no encontrado");

            var result = await _expenseService.DeleteExpenseAsync(id);
            if (!result) return StatusCode(500, "Error al eliminar el gasto");

            return NoContent();
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
            var expenses = await _expenseService.GetExpensesByTypeAsync(expenseType);
            return Ok(expenses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}