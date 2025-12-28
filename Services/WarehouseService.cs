namespace MiniMazErpBack;

public class WarehouseService(WarehouseRepository repo) : IWarehouseService
{
    private readonly WarehouseRepository _repo = repo;
    public async Task<Warehouse> CreateWarehouseAsync(CreateWarehouseDto warehouseDto)
    {
        try
        {
            var warehouse = new Warehouse()
            {
                Name = warehouseDto.Name,
                Description = warehouseDto.Description
            };
            await _repo.CreateAsync(warehouse);

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
            var warehouse = await _repo.GetByIdAsync(id);
            ArgumentNullException.ThrowIfNull(warehouse);
            await _repo.DeleteAsync(id);

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
            var warehouses = await _repo.GetAllAsync();
            return warehouses.ToList();
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
            return await _repo.GetByIdAsync(id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateWarehouseAsync(UpdateWarehouseDto warehouseDto)
    {
        try
        {
            var warehouse = new Warehouse()
            {
                Name = warehouseDto.Name,
                Description = warehouseDto.Description
            };
            await _repo.UpdateAsync(warehouse);
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
