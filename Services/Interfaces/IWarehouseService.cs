namespace MiniMazErpBack;

public interface IWarehouseService
{
    Task<Warehouse> CreateWarehouseAsync(CreateWarehouseDto warehouseDto);
    Task<bool> DeleteWarehouseAsync(int id);
    Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
    Task<Warehouse?> GetWarehouseByIdAsync(int id);
    Task<bool> UpdateWarehouseAsync(UpdateWarehouseDto warehouseDto);
}
