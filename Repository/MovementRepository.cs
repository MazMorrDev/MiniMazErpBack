namespace MiniMazErpBack;

public class MovementRepository : IMovementRepository
{
    public Task<int> CreateAsync(Movement movement)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Movement>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Movement?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Movement movement)
    {
        throw new NotImplementedException();
    }
}
