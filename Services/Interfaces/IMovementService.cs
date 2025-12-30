namespace MiniMazErpBack;

public interface IMovementService
{
    Task<Movement> CreateMovementAsync(CreateMovementDto movementDto);
    Task<bool> DeleteMovementAsync(int id);
    Task<IEnumerable<Movement>> GetAllMovementsAsync();
    Task<Movement?> GetMovementByIdAsync(int id);
    Task<bool> UpdateMovementAsync(int id, UpdateMovementDto movementDto);
    Task<bool> HasRelatedRecordsAsync(int movementId);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Movement>> GetByProductIdAsync(int productId);
    Task<IEnumerable<Movement>> GetByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate);
}
