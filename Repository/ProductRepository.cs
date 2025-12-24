namespace MiniMazErpBack;

public class ProductRepository : IProductRepository
{
    public Task<int> CreateAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Product product)
    {
        throw new NotImplementedException();
    }
}
