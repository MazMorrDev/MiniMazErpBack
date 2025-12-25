namespace MiniMazErpBack;

public interface IExpenseService
{
    Task<Expense> CreateExpenseAsync(Expense expense);
    Task<bool> DeleteExpenseAsync(int id);
    Task<IEnumerable<Expense>> GetAllExpenseAsync();
    Task<Expense?> GetExpenseByIdAsync(int id);
    Task<bool> UpdateExpenseAsync(Expense expense);
}
