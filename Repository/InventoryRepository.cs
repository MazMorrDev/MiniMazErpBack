namespace MiniMazErpBack;

public class InventoryRepository : IInventoryRepository
{
    public Task<int> CreateAsync(Inventory inventory)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Inventory>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Inventory?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Inventory inventory)
    {
        throw new NotImplementedException();
    }
}
