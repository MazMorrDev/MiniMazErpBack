namespace MiniMazErpBack;

public interface IInventoryService
{
    Task<Inventory> CreateInventoryAsync(Inventory inventory);
    Task<bool> DeleteInventoryAsync(int id);
    Task<IEnumerable<Inventory>> GetAllInventoriesAsync();
    Task<Inventory?> GetInventoryByIdAsync(int id);
    Task<bool> UpdateInventoryAsync(Inventory inventory);
}
