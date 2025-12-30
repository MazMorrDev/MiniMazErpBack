namespace MiniMazErpBack;

public interface IExpenseService
{
    Task<Expense> CreateExpenseAsync(CreateExpenseDto expenseDto);
    Task<bool> DeleteExpenseAsync(int id);
    Task<IEnumerable<Expense>> GetAllExpensesAsync();
    Task<Expense?> GetExpenseByIdAsync(int id);
    Task<bool> UpdateExpenseAsync(int id, UpdateExpenseDto expenseDto);
    Task<Expense?> GetFullExpenseByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Expense>> GetExpensesByTypeAsync(ExpenseType expenseType);
    Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate);
}
