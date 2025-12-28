namespace MiniMazErpBack;

public class SellService(ISellRepository sellRepo, IMovementRepository movementRepo) : ISellService
{
    private readonly ISellRepository _sellRepo = sellRepo;
    private readonly IMovementRepository _movementRepo = movementRepo;

    public async Task<Sell> CreateSellAsync(Sell sell)
    {
        if (sell.Movement == null)
            throw new ArgumentException("El objeto Sell debe tener un Movement asociado");

        // Crear Movement primero
        var movementId = await _movementRepo.CreateAsync(sell.Movement);

        // Crear Sell con el mismo ID
        sell.MovementId = movementId;
        sell.Movement.Id = movementId;

        await _sellRepo.CreateAsync(sell);
        return sell;
    }

    public async Task<bool> DeleteSellAsync(int id)
    {
        return await _sellRepo.DeleteAsync(id);
    }

    public async Task<IEnumerable<Sell>> GetAllSellsAsync()
    {
        return await _sellRepo.GetAllAsync();
    }

    public async Task<Sell?> GetSellByIdAsync(int id)
    {
        return await _sellRepo.GetByIdAsync(id);
    }

    public async Task<bool> UpdateSellAsync(Sell sell)
    {
        if (sell.Movement != null)
        {
            await _movementRepo.UpdateAsync(sell.Movement);
        }

        return await _sellRepo.UpdateAsync(sell);
    }

    // Métodos adicionales del repositorio Sell
    public async Task<IEnumerable<Sell>> GetSellsByProductIdAsync(int productId)
    {
        if (_sellRepo is SellRepository sellRepository)
        {
            return await sellRepository.GetByProductIdAsync(productId);
        }
        throw new InvalidOperationException("El repositorio no es del tipo esperado");
    }

    public async Task<IEnumerable<Sell>> GetSellsByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (_sellRepo is SellRepository sellRepository)
        {
            return await sellRepository.GetByDateRangeAsync(startDate, endDate);
        }
        throw new InvalidOperationException("El repositorio no es del tipo esperado");
    }

    // Método para obtener Sell completo
    public async Task<Sell?> GetFullSellByIdAsync(int id)
    {
        var sell = await _sellRepo.GetByIdAsync(id);
        if (sell == null) return null;

        // El repositorio Sell ya carga Movement en GetByIdAsync
        return sell;
    }
}