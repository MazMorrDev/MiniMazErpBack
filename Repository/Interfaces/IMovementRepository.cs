namespace MiniMazErpBack;

public interface IMovementRepository
{
    Task<IEnumerable<Movement>> GetAllAsync();
    Task<Movement?> GetByIdAsync(int id);
    Task<int> CreateAsync(Movement movement);
    Task<bool> UpdateAsync(Movement movement);
    Task<bool> DeleteAsync(int id);
}
