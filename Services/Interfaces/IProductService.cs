namespace MiniMazErpBack;

public interface IProductService
{
    Task<Product> CreateProductAsync(CreateProductDto productDto);
    Task<bool> DeleteProductAsync(int id);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<bool> UpdateProductAsync(int id, UpdateProductDto productDto);
}
