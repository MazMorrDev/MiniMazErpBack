namespace MiniMazErpBack;

public interface IBuyService
{
    Task<Buy> CreateBuyAsync(CreateBuyDto buyDto);
    Task<bool> DeleteBuyAsync(int id);
    Task<IEnumerable<Buy>> GetAllBuysAsync();
    Task<Buy?> GetBuyByIdAsync(int id);
    Task<bool> UpdateBuyAsync(int id, UpdateBuyDto buyDto);
}
