namespace MiniMazErpBack;

public interface IInventoryRepository
{
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory?> GetByIdAsync(int id);
        Task<int> CreateAsync(Inventory inventory);
        Task<bool> UpdateAsync(Inventory inventory);
        Task<bool> DeleteAsync(int id);
}
