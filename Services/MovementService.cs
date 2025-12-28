﻿namespace MiniMazErpBack;

public class MovementService(MovementRepository movementRepo, BuyRepository buyRepo, 
                            SellRepository sellRepo, ExpenseRepository expenseRepo) : IMovementService
{
    private readonly MovementRepository _movementRepo = movementRepo;
    private readonly BuyRepository _buyRepo = buyRepo;
    private readonly SellRepository _sellRepo = sellRepo;
    private readonly ExpenseRepository _expenseRepo = expenseRepo;

    public async Task<Movement> CreateMovementAsync(Movement movement)
    {
        var movementId = await _movementRepo.CreateAsync(movement);
        movement.Id = movementId;
        return movement;
    }

    public async Task<bool> DeleteMovementAsync(int id)
    {
        // Verificar si tiene registros hijos primero
        var hasRelatedRecords = await HasRelatedRecordsAsync(id);
        if (hasRelatedRecords)
        {
            throw new InvalidOperationException("No se puede eliminar un movimiento que tiene registros relacionados (Buy/Sell/Expense)");
        }

        return await _movementRepo.DeleteAsync(id);
    }

    public async Task<IEnumerable<Movement>> GetAllMovementsAsync()
    {
        return await _movementRepo.GetAllAsync();
    }

    public async Task<Movement?> GetMovementByIdAsync(int id)
    {
        return await _movementRepo.GetByIdAsync(id);
    }

    public async Task<bool> UpdateMovementAsync(Movement movement)
    {
        // Verificar si tiene registros hijos antes de actualizar
        var hasRelatedRecords = await HasRelatedRecordsAsync(movement.Id);
        if (hasRelatedRecords)
        {
            throw new InvalidOperationException("No se puede actualizar un movimiento que tiene registros relacionados (Buy/Sell/Expense)");
        }

        return await _movementRepo.UpdateAsync(movement);
    }

    // Método para verificar si un Movement tiene registros hijos
    public async Task<bool> HasRelatedRecordsAsync(int movementId)
    {
        // Verificar en todas las tablas hijas
        var hasBuy = await _buyRepo.ExistsAsync(movementId);
        var hasSell = await _sellRepo.ExistsAsync(movementId);
        var hasExpense = await _expenseRepo.ExistsAsync(movementId);

        return hasBuy || hasSell || hasExpense;
    }

    // NUEVO: Método para verificar si existe un movimiento
    public async Task<bool> ExistsAsync(int id)
    {
        return await _movementRepo.ExistsAsync(id);
    }

    // NUEVO: Método para obtener movimientos por producto
    public async Task<IEnumerable<Movement>> GetByProductIdAsync(int productId)
    {
        return await _movementRepo.GetByProductIdAsync(productId);
    }

    // NUEVO: Método para obtener movimientos por rango de fechas
    public async Task<IEnumerable<Movement>> GetByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await _movementRepo.GetByDateRangeAsync(startDate, endDate);
    }
}