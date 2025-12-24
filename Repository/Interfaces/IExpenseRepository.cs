namespace MiniMazErpBack;

public interface IExpenseRepository
{
    Task<IEnumerable<Expense>> GetAllAsync();
    Task<Expense?> GetByIdAsync(int id);
    Task<int> CreateAsync(Expense expense);
    Task<bool> UpdateAsync(Expense expense);
    Task<bool> DeleteAsync(int id);
}
