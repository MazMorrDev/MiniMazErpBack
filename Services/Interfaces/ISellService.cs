namespace MiniMazErpBack;

public interface ISellService
{
    Task<Sell> CreateSellAsync(Sell sell);
    Task<bool> DeleteSellAsync(int id);
    Task<IEnumerable<Sell>> GetAllSellsAsync();
    Task<Sell?> GetSellByIdAsync(int id);
    Task<bool> UpdateSellAsync(Sell sell);
}
