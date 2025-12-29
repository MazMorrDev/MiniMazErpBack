namespace MiniMazErpBack;

public class BuyService(AppDbContext context) : IBuyService
{
    private readonly AppDbContext _context = context;

    public async Task<Buy> CreateBuyAsync(Buy buy)
    {
        // Validar que el objeto Buy tiene un Movement válido
        if (buy.Movement == null)
            throw new ArgumentException("El objeto Buy debe tener un Movement asociado");

        // Primero crear el Movement
        var movementId = await _movementRepo.CreateAsync(buy.Movement);

        // Asignar el ID al Buy y crear el registro específico
        buy.MovementId = movementId;
        buy.Movement.Id = movementId;

        await _buyRepo.CreateAsync(buy);
        return buy;
    }

    public async Task<bool> DeleteBuyAsync(int id)
    {
        // Solo elimina de Buy, Movement se mantiene (o puedes implementar cascada si es necesario)
        return await _buyRepo.DeleteAsync(id);
    }

    public async Task<IEnumerable<Buy>> GetAllBuysAsync()
    {
        return await _buyRepo.GetAllAsync();
    }

    public async Task<Buy?> GetBuyByIdAsync(int id)
    {
        return await _buyRepo.GetByIdAsync(id);
    }

    public async Task<bool> UpdateBuyAsync(Buy buy)
    {
        // Primero actualizar el Movement si es necesario
        if (buy.Movement != null)
        {
            await _movementRepo.UpdateAsync(buy.Movement);
        }

        // Luego actualizar el Buy
        return await _buyRepo.UpdateAsync(buy);
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