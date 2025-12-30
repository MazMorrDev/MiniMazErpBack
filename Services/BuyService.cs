using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class BuyService(AppDbContext context, MovementService movementService) : IBuyService
{
    private readonly AppDbContext _context = context;
    private readonly MovementService _movementService = movementService;

    public async Task<Buy> CreateBuyAsync(CreateBuyDto buyDto, CreateMovementDto movementDto)
    {
        try
        {
            // Crear el Movement
            var newMovement = await _movementService.CreateMovementAsync(movementDto);
            await _context.Movements.AddAsync(newMovement);

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
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<bool> DeleteBuyAsync(int id)
    {
        try
        {
            var buy = await _context.Buys.FindAsync(id);
            ArgumentNullException.ThrowIfNull(buy);
            var movement = buy.Movement;

            _context.Buys.Remove(buy);
            _context.Movements.Remove(movement);

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }

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
            WarehouseId = buyDto.WarehouseId,
            ProductId = buyDto.ProductId,
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
        var buy = await _buyRepo.GetByIdAsync(id);
        if (buy == null) return null;

        // Cargar el Movement completo
        var movement = await _movementRepo.GetByIdAsync(id);
        if (movement != null)
        {
            buy.Movement = movement;
        }

        return buy;
    }

    // NUEVO: Método para eliminar completamente (Buy + Movement)
    public async Task<bool> DeleteBuyAndMovementAsync(int id)
    {
        // Eliminar Buy primero
        var buyDeleted = await _buyRepo.DeleteAsync(id);
        if (!buyDeleted) return false;

        // Luego eliminar Movement
        return await _movementRepo.DeleteAsync(id);
    }

    // NUEVO: Método para verificar si existe una compra
    public async Task<bool> ExistsAsync(int id)
    {
        return await _buyRepo.ExistsAsync(id);
    }

    // NUEVO: Método para obtener compras por producto
    public async Task<IEnumerable<Buy>> GetBuysByProductIdAsync(int productId)
    {
        return await _buyRepo.GetByProductIdAsync(productId);
    }

    // NUEVO: Método para obtener compras por rango de fechas
    public async Task<IEnumerable<Buy>> GetBuysByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await _buyRepo.GetByDateRangeAsync(startDate, endDate);
    }
}