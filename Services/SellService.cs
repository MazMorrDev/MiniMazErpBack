// SellService.cs
using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class SellService(AppDbContext context, MovementService movementService) : ISellService
{
    private readonly AppDbContext _context = context;
    private readonly MovementService _movementService = movementService;

    public async Task<Sell> CreateSellAsync(CreateSellDto sellDto)
    {
        try
        {
            // Crear el Movement
            var movementDto = new CreateMovementDto()
            {
                InventoryId = sellDto.InventoryId,
                ProductId = sellDto.ProductId,
                Description = sellDto.Description,
                Quantity = sellDto.Quantity,
                MovementDate = sellDto.MovementDate
            };

            var newMovement = await _movementService.CreateMovementAsync(movementDto);

            // Crear el nuevo Sell
            var sell = new Sell()
            {
                MovementId = newMovement.Id,
                Movement = newMovement,
                SalePrice = sellDto.SalePrice,
                DiscountPercentage = sellDto.DiscountPercentage
            };

            await _context.Sells.AddAsync(sell);
            await _context.SaveChangesAsync();
            return sell;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> DeleteSellAsync(int id)
    {
        try
        {
            var sell = await _context.Sells
                .Include(s => s.Movement)
                .FirstOrDefaultAsync(s => s.MovementId == id);

            if (sell == null) return false;

            var movement = sell.Movement;

            _context.Sells.Remove(sell);
            _context.Movements.Remove(movement);

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<Sell>> GetAllSellsAsync()
    {
        return await _context.Sells.ToListAsync();
    }

    public async Task<Sell?> GetSellByIdAsync(int id)
    {
        return await _context.Sells.FindAsync(id);
    }

    public async Task<bool> UpdateSellAsync(int id, UpdateSellDto sellDto)
    {
        var movementDto = new UpdateMovementDto()
        {
            InventoryId = sellDto.InventoryId,
            ProductId = sellDto.ProductId,
            Description = sellDto.Description,
            Quantity = sellDto.Quantity,
            MovementDate = sellDto.MovementDate
        };
        
        // Actualizar el movement 
        await _movementService.UpdateMovementAsync(id, movementDto);

        // Actualizar el sell
        var sell = await _context.Sells.FindAsync(id);
        ArgumentNullException.ThrowIfNull(sell);
        sell.SalePrice = sellDto.SalePrice;
        sell.DiscountPercentage = sellDto.DiscountPercentage;

        // Mandar los cambios
        await _context.SaveChangesAsync();
        return true;
    }

    // Método adicional: Obtener Sell completo con Movement cargado
    public async Task<Sell?> GetFullSellByIdAsync(int id)
    {
        try
        {
            return await _context.Sells
                .Include(s => s.Movement)
                .FirstOrDefaultAsync(s => s.MovementId == id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    // Método para verificar si existe un sell
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Sells.AnyAsync(s => s.MovementId == id);
    }

    // Método para obtener sells por producto
    public async Task<IEnumerable<Sell>> GetSellsByProductIdAsync(int productId)
    {
        try
        {
            return await _context.Sells
                .Include(s => s.Movement)
                .Where(s => s.Movement != null && s.Movement.ProductId == productId)
                .ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<Sell>> GetSellsByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await _context.Sells
            .Include(s => s.Movement)
            .ThenInclude(m => m.Product)
            .Include(s => s.Movement)
            .ThenInclude(m => m.Inventory)
            .Where(s => s.Movement.MovementDate >= startDate && s.Movement.MovementDate <= endDate)
            .OrderByDescending(s => s.Movement.MovementDate)
            .ToListAsync();
    }
}