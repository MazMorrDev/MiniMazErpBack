using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class WarehouseService(AppDbContext context) : IWarehouseService
{
    private readonly AppDbContext _context = context;
    public async Task<Warehouse> CreateWarehouseAsync(CreateWarehouseDto warehouseDto)
    {
        try
        {
            var client = await _context.Clients.FindAsync(warehouseDto.ClientId);
            ArgumentNullException.ThrowIfNull(client);

            var warehouse = new Warehouse()
            {
                ClientId = warehouseDto.ClientId,
                Name = warehouseDto.Name,
                Description = warehouseDto.Description,
                Client = client
            };
            await _context.Warehouses.AddAsync(warehouse);
            await _context.SaveChangesAsync();
            return warehouse;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> DeleteWarehouseAsync(int id)
    {
        try
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            ArgumentNullException.ThrowIfNull(warehouse);

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
    {
        try
        {
            return await _context.Warehouses.ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Warehouse?> GetWarehouseByIdAsync(int id)
    {
        try
        {
            return await _context.Warehouses.FindAsync(id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateWarehouseAsync(int id, UpdateWarehouseDto warehouseDto)
    {
        try
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            ArgumentNullException.ThrowIfNull(warehouse);

            warehouse.Name = warehouseDto.Name;
            warehouse.Description = warehouseDto.Description;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
