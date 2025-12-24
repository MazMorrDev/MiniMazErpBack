namespace MiniMazErpBack;

public class ExpenseRepository : IExpenseRepository
{
    public Task<int> CreateAsync(Expense expense)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Expense>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Expense?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Expense expense)
    {
        throw new NotImplementedException();
    }
}
