namespace MiniMazErpBack;

public class ExpenseService(AppDbContext context) : IExpenseService
{
    private readonly AppDbContext _context = context;

    public async Task<Expense> CreateExpenseAsync(Expense expense)
    {
        if (expense.Movement == null)
            throw new ArgumentException("El objeto Expense debe tener un Movement asociado");

        // Crear Movement primero
        var movementId = await _movementRepo.CreateAsync(expense.Movement);

        // Crear Expense con el mismo ID
        expense.MovementId = movementId;
        expense.Movement.Id = movementId;

        await _expenseRepo.CreateAsync(expense);
        return expense;
    }

    public async Task<bool> DeleteExpenseAsync(int id)
    {
        // Eliminar solo Expense (no Movement para mantener integridad histórica)
        return await _expenseRepo.DeleteAsync(id);
    }

    public async Task<IEnumerable<Expense>> GetAllExpenseAsync()
    {
        return await _expenseRepo.GetAllAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int id)
    {
        return await _expenseRepo.GetByIdAsync(id);
    }

    public async Task<bool> UpdateExpenseAsync(Expense expense)
    {
        // Actualizar Movement si es necesario
        if (expense.Movement != null)
        {
            await _movementRepo.UpdateAsync(expense.Movement);
        }

        return await _expenseRepo.UpdateAsync(expense);
    }

    // Método para eliminar completamente (Expense + Movement)
    public async Task<bool> DeleteExpenseAndMovementAsync(int id)
    {
        // Eliminar Expense primero
        var expenseDeleted = await _expenseRepo.DeleteAsync(id);
        if (!expenseDeleted) return false;

        // Luego eliminar Movement
        return await _movementRepo.DeleteAsync(id);
    }

    // Método para obtener Expense con Movement cargado
    public async Task<Expense?> GetFullExpenseByIdAsync(int id)
    {
        var expense = await _expenseRepo.GetByIdAsync(id);
        if (expense == null) return null;

        var movement = await _movementRepo.GetByIdAsync(id);
        if (movement != null)
        {
            expense.Movement = movement;
        }

        return expense;
    }

    // NUEVO: Método para verificar si existe un gasto
    public async Task<bool> ExistsAsync(int id)
    {
        return await _expenseRepo.ExistsAsync(id);
    }

    // NUEVO: Método para obtener gastos por tipo
    public async Task<IEnumerable<Expense>> GetByTypeAsync(ExpenseType expenseType)
    {
        return await _expenseRepo.GetByTypeAsync(expenseType);
    }

    // NUEVO: Método para obtener gastos por rango de fechas
    public async Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await _expenseRepo.GetByDateRangeAsync(startDate, endDate);
    }
}
