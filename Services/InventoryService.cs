using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class InventoryService(AppDbContext context) : IInventoryService
{
    private readonly AppDbContext _context = context;

    public async Task<Inventory> CreateInventoryAsync(CreateInventoryDto inventoryDto)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(inventoryDto);

            var inventory = new Inventory()
            {
                ClientId = inventoryDto.ClientId,
                ProductId = inventoryDto.ProductId,
                Stock = inventoryDto.Stock
            };
            await _context.Inventories.AddAsync(inventory);
            await _context.SaveChangesAsync();
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
            var inventory = await _context.Inventories.FindAsync(id);
            ArgumentNullException.ThrowIfNull(inventory);

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
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
            return await _context.Inventories.ToListAsync();
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
            return await _context.Inventories.FindAsync(id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateInventoryAsync(int id, UpdateInventoryDto inventoryDto)
    {
        try
        {
            var inventory = await _context.Inventories.FindAsync(id);
            ArgumentNullException.ThrowIfNull(inventory);

            inventory.ClientId = inventoryDto.ClientId;
            inventory.ProductId = inventoryDto.ProductId;
            inventory.Stock = inventoryDto.Stock;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
