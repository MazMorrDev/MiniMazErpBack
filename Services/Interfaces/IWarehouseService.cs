namespace MiniMazErpBack;

public interface IWarehouseService
{
    Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse);
    Task<bool> DeleteWarehouseAsync(int id);
    Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
    Task<Warehouse?> GetWarehouseByIdAsync(int id);
    Task<bool> UpdateWarehouseAsync(Warehouse warehouse);
}
