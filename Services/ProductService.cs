namespace MiniMazErpBack;

public class ProductService(ProductRepository repo) : IProductService
{
    private readonly ProductRepository _repo = repo;
    public async Task<Product> CreateProductAsync(CreateProductDto productDto)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(productDto);
            
            var product = new Product()
            {
                Name = productDto.Name,
                SellPrice = productDto.SellPrice
            };

            await _repo.CreateAsync(product);
            return product;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _repo.GetByIdAsync(id);
            ArgumentNullException.ThrowIfNull(product);

            await _repo.DeleteAsync(id);

            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        try
        {
            return await _repo.GetAllAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        try
        {
            return await _repo.GetByIdAsync(id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateProductAsync(UpdateProductDto productDto)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(productDto);

            var product = new Product()
            {
                Name = productDto.Name,
                SellPrice = productDto.SellPrice
            };

            await _repo.UpdateAsync(product);
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
