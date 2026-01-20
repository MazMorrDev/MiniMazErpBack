using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class BuyService(AppDbContext context, IMovementService movementService) : IBuyService
{
    private readonly AppDbContext _context = context;
    private readonly IMovementService _movementService = movementService;

    public async Task<Buy> CreateBuyAsync(CreateBuyDto buyDto)
    {

        // Crear el Movement
        var movementDto = new CreateMovementDto()
        {
            InventoryId = buyDto.InventoryId,
            Description = buyDto.Description,
            Quantity = buyDto.Quantity,
            MovementDate = buyDto.MovementDate
        };

        var newMovement = await _movementService.CreateMovementAsync(movementDto);

        // Crear el nuevo Buy
        var buy = new Buy()
        {
            MovementId = newMovement.Id,
            Movement = newMovement,
            UnitPrice = buyDto.UnitPrice
        };

        await _context.Buys.AddAsync(buy);

        await _context.SaveChangesAsync();
        return buy;
    }

    public async Task<bool> DeleteBuyAsync(int id)
    {
        var buy = await _context.Buys
            .Include(b => b.Movement)
            .FirstOrDefaultAsync(b => b.MovementId == id);

        if (buy == null) return false;

        var movement = buy.Movement;

        _context.Buys.Remove(buy);
        _context.Movements.Remove(movement);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Buy>> GetAllBuysAsync()
    {
        return await _context.Buys.ToListAsync();
    }

    public async Task<Buy?> GetBuyByIdAsync(int id)
    {
        return await _context.Buys.FindAsync(id);
    }

    public async Task<bool> UpdateBuyAsync(int id, UpdateBuyDto buyDto)
    {
        var movementDto = new UpdateMovementDto()
        {
            InventoryId = buyDto.InventoryId,
            Description = buyDto.Description,
            Quantity = buyDto.Quantity,
            MovementDate = buyDto.MovementDate
        };
        // Actualizar el movement 
        await _movementService.UpdateMovementAsync(id, movementDto);

        // Actualizar el buy
        var buy = await _context.Buys.FindAsync(id);
        ArgumentNullException.ThrowIfNull(buy);
        buy.UnitPrice = buyDto.UnitPrice;

        // Mandar los cambios
        await _context.SaveChangesAsync();
        return true;
    }

    // Método adicional: Obtener Buy completo con Movement cargado
    public async Task<Buy?> GetFullBuyByIdAsync(int id)
    {
        return await _context.Buys
            .Include(b => b.Movement)
            .FirstOrDefaultAsync(b => b.MovementId == id);
    }

    // NUEVO: Método para verificar si existe una compra
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Buys.AnyAsync(b => b.MovementId == id);
    }

    // NUEVO: Método para obtener compras por producto
    public async Task<IEnumerable<Buy>> GetBuysByProductIdAsync(int productId)
    {
        return await _context.Buys
            .Include(b => b.Movement)
            .Where(b => b.Movement != null && b.Movement.Inventory.ProductId == productId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Buy>> GetBuysByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await _context.Buys
            .Include(b => b.Movement)
            .Where(b => b.Movement.MovementDate >= startDate && b.Movement.MovementDate <= endDate)
            .OrderByDescending(b => b.Movement.MovementDate) // Ordenar por fecha más reciente primero
            .ToListAsync();
    }
}