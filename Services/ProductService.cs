using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class ProductService(AppDbContext context) : IProductService
{
    private readonly AppDbContext _context = context;
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

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

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
            var product = await _context.Products.FindAsync(id);
            ArgumentNullException.ThrowIfNull(product);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

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
            return await _context.Products.ToListAsync();
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
            return await _context.Products.FindAsync(id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateProductAsync(int id, UpdateProductDto productDto)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(productDto);
            var product = await _context.Products.FindAsync(id);
            ArgumentNullException.ThrowIfNull(product);
            product.Name = productDto.Name;
            product.SellPrice = productDto.SellPrice;
            
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
