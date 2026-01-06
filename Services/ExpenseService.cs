using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class ExpenseService(AppDbContext context, MovementService movementService) : IExpenseService
{
    private readonly AppDbContext _context = context;
    private readonly MovementService _movementService = movementService;

    public async Task<Expense> CreateExpenseAsync(CreateExpenseDto expenseDto)
    {

        // Crear el Movement
        var movementDto = new CreateMovementDto()
        {
            InventoryId = expenseDto.InventoryId,
            ProductId = expenseDto.ProductId,
            Description = expenseDto.Description,
            Quantity = expenseDto.Quantity,
            MovementDate = expenseDto.MovementDate
        };

        var newMovement = await _movementService.CreateMovementAsync(movementDto);

        // Crear el nuevo Expense
        var expense = new Expense()
        {
            MovementId = newMovement.Id,
            Movement = newMovement,
            ExpenseType = expenseDto.ExpenseType,
            TotalPrice = expenseDto.TotalPrice
        };

        await _context.Expenses.AddAsync(expense);
        await _context.SaveChangesAsync();
        return expense;

    }

    public async Task<bool> DeleteExpenseAsync(int id)
    {

        var expense = await _context.Expenses
            .Include(e => e.Movement)
            .FirstOrDefaultAsync(e => e.MovementId == id);

        if (expense == null) return false;

        var movement = expense.Movement;

        _context.Expenses.Remove(expense);
        _context.Movements.Remove(movement);

        await _context.SaveChangesAsync();
        return true;

    }

    public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
    {
        return await _context.Expenses.ToListAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int id)
    {
        return await _context.Expenses.FindAsync(id);
    }

    public async Task<bool> UpdateExpenseAsync(int id, UpdateExpenseDto expenseDto)
    {
        var movementDto = new UpdateMovementDto()
        {
            InventoryId = expenseDto.InventoryId,
            ProductId = expenseDto.ProductId,
            Description = expenseDto.Description,
            Quantity = expenseDto.Quantity,
            MovementDate = expenseDto.MovementDate
        };

        // Actualizar el movement 
        await _movementService.UpdateMovementAsync(id, movementDto);

        // Actualizar el expense
        var expense = await _context.Expenses.FindAsync(id);
        ArgumentNullException.ThrowIfNull(expense);
        expense.ExpenseType = expenseDto.ExpenseType;
        expense.TotalPrice = expenseDto.TotalPrice;

        // Mandar los cambios
        await _context.SaveChangesAsync();
        return true;
    }

    // Método adicional: Obtener Expense completo con Movement cargado
    public async Task<Expense?> GetFullExpenseByIdAsync(int id)
    {
        return await _context.Expenses
            .Include(e => e.Movement)
            .FirstOrDefaultAsync(e => e.MovementId == id);
    }

    // Método para verificar si existe un expense
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Expenses.AnyAsync(e => e.MovementId == id);
    }

    // Método para obtener expenses por tipo
    public async Task<IEnumerable<Expense>> GetExpensesByTypeAsync(ExpenseType expenseType)
    {
        try
        {
            return await _context.Expenses
                .Include(e => e.Movement)
                .Where(e => e.ExpenseType == expenseType)
                .ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await _context.Expenses
            .Include(e => e.Movement)
            .ThenInclude(m => m.Product)
            .Include(e => e.Movement)
            .ThenInclude(i => i.Inventory)
            .Where(e => e.Movement.MovementDate >= startDate && e.Movement.MovementDate <= endDate)
            .OrderByDescending(e => e.Movement.MovementDate)
            .ToListAsync();
    }
}