namespace MiniMazErpBack;

public interface IWarehouseRepository
{
        Task<IEnumerable<Warehouse>> GetAllAsync();
        Task<Warehouse?> GetByIdAsync(int id);
        Task<int> CreateAsync(Warehouse warehouse);
        Task<bool> UpdateAsync(Warehouse warehouse);
        Task<bool> DeleteAsync(int id);
}
