namespace MiniMazErpBack;

public interface ISellService
{
    Task<Sell> CreateSellAsync(CreateSellDto sellDto);
    Task<bool> DeleteSellAsync(int id);
    Task<IEnumerable<Sell>> GetAllSellsAsync();
    Task<Sell?> GetSellByIdAsync(int id);
    Task<bool> UpdateSellAsync(int id, UpdateSellDto sellDto);
    Task<Sell?> GetFullSellByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Sell>> GetSellsByProductIdAsync(int productId);
    Task<IEnumerable<Sell>> GetSellsByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate);
}
