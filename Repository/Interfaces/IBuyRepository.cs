namespace MiniMazErpBack;

public interface IBuyRepository
{
        Task<IEnumerable<Buy>> GetAllAsync();
        Task<Buy?> GetByIdAsync(int id);
        Task<int> CreateAsync(Buy buy);
        Task<bool> UpdateAsync(Buy buy);
        Task<bool> DeleteAsync(int id);
}
