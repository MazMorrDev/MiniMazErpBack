namespace MiniMazErpBack;

public interface ISellRepository
{
        Task<IEnumerable<Sell>> GetAllAsync();
        Task<Sell?> GetByIdAsync(int id);
        Task<int> CreateAsync(Sell sell);
        Task<bool> UpdateAsync(Sell sell);
        Task<bool> DeleteAsync(int id);
}
