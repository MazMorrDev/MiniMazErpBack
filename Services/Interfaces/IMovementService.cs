namespace MiniMazErpBack;

public interface IMovementService
{
    Task<Movement> CreateMovementAsync(Movement movement);
    Task<bool> DeleteMovementAsync(int id);
    Task<IEnumerable<Movement>> GetAllMovementsAsync();
    Task<Movement?> GetMovementByIdAsync(int id);
    Task<bool> UpdateMovementAsync(Movement movement);
}
