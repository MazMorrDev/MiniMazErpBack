namespace MiniMazErpBack;

public interface IInventoryService
{
    Task<Inventory> CreateInventoryAsync(CreateInventoryDto inventoryDto);
    Task<bool> DeleteInventoryAsync(int id);
    Task<IEnumerable<Inventory>> GetAllInventoriesAsync();
    Task<Inventory?> GetInventoryByIdAsync(int id);
    Task<bool> UpdateInventoryAsync(int id, UpdateInventoryDto inventoryDto);
}
