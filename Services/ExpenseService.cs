namespace MiniMazErpBack;

public class ExpenseService : IExpenseService
{
    public Task<Expense> CreateExpenseAsync(Expense expense)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteExpenseAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Expense>> GetAllExpenseAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Expense?> GetExpenseByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateExpenseAsync(Expense expense)
    {
        throw new NotImplementedException();
    }
}
