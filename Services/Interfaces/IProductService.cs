namespace MiniMazErpBack;

public interface IProductService
{
    Task<Product> CreateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<bool> UpdateProductAsync(Product product);
}
