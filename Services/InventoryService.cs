using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MiniMazErpBack;

public class InventoryService(InventoryRepository repo) : IInventoryService
{
    private readonly InventoryRepository _repo = repo;

    public async Task<Inventory> CreateInventoryAsync(CreateInventoryDto inventoryDto)
    {
        try
        {
            var inventory = new Inventory()
            {
                WarehouseId = inventoryDto.WarehouseId,
                ProductId = inventoryDto.ProductId,
                Stock = inventoryDto.Stock,
                AlertStock = inventoryDto.AlertStock,
                WarningStock = inventoryDto.WarningStock
            };
            await _repo.CreateAsync(inventory);

            return inventory;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> DeleteInventoryAsync(int id)
    {
        try
        {
            var inventory = _repo.GetByIdAsync(id);
            ArgumentNullException.ThrowIfNull(inventory);
            await _repo.DeleteAsync(id);
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<Inventory>> GetAllInventoriesAsync()
    {
        try
        {
            return await _repo.GetAllAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Inventory?> GetInventoryByIdAsync(int id)
    {
        try
        {
            return await _repo.GetByIdAsync(id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateInventoryAsync(UpdateInventoryDto inventoryDto)
    {
        try
        {
            var inventory = new Inventory()
            {
                WarehouseId = inventoryDto.WarehouseId,
                ProductId = inventoryDto.ProductId,
                Stock = inventoryDto.Stock,
                AlertStock = inventoryDto.AlertStock,
                WarningStock = inventoryDto.WarningStock
            };
            await _repo.UpdateAsync(inventory);
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
