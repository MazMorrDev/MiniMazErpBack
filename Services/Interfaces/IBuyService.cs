namespace MiniMazErpBack;

public interface IBuyService
{
    Task<Buy> CreateBuyAsync(Buy buy);
    Task<bool> DeleteBuyAsync(int id);
    Task<IEnumerable<Buy>> GetAllBuysAsync();
    Task<Buy?> GetBuyByIdAsync(int id);
    Task<bool> UpdateBuyAsync(Buy buy);
}
