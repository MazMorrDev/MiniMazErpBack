﻿using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class MovementService(AppDbContext context) : IMovementService
{
    private readonly AppDbContext _context = context;

    public async Task<Movement> CreateMovementAsync(CreateMovementDto movementDto)
    {
        try
        {
            // Validar que Warehouse existe
            var warehouseExists = await _context.Warehouses.AnyAsync(w => w.ClientId == movementDto.WarehouseId);
            if (!warehouseExists) throw new ArgumentException($"Warehouse con ID {movementDto.WarehouseId} no existe");

            // Validar que Product existe
            var productExists = await _context.Products.AnyAsync(p => p.Id == movementDto.ProductId);
            if (!productExists) throw new ArgumentException($"Producto con ID {movementDto.ProductId} no existe");

            // Validar cantidad
            if (movementDto.Quantity == 0) throw new ArgumentException("La cantidad no puede ser 0");

            var movement = new Movement()
            {
                WarehouseId = movementDto.WarehouseId,
                ProductId = movementDto.ProductId,
                Description = movementDto.Description,
                Quantity = movementDto.Quantity,
                MovementDate = movementDto.MovementDate,
            };

            await _context.Movements.AddAsync(movement);
            await _context.SaveChangesAsync();

            // Cargar relaciones para retornar objeto completo
            await _context.Entry(movement).Reference(m => m.Warehouse).LoadAsync();
            await _context.Entry(movement).Reference(m => m.Product).LoadAsync();

            return movement;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<bool> DeleteMovementAsync(int id)
    {
        try
        {
            // Verificar si existe
            var movement = await _context.Movements.FindAsync(id);
            if (movement == null)
            {
                return false;
            }

            // Verificar si tiene registros hijos
            var hasRelatedRecords = await HasRelatedRecordsAsync(id);
            if (hasRelatedRecords)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar un movimiento que tiene registros relacionados (Buy/Sell/Expense)");
            }

            _context.Movements.Remove(movement);
            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<IEnumerable<Movement>> GetAllMovementsAsync()
    {
        try
        {
            return await _context.Movements
                .Include(m => m.Warehouse)
                .Include(m => m.Product)
                .OrderByDescending(m => m.MovementDate)
                .ThenByDescending(m => m.Id)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Movement?> GetMovementByIdAsync(int id)
    {
        try
        {
            return await _context.Movements
                .Include(m => m.Warehouse)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateMovementAsync(int id, UpdateMovementDto movementDto)
    {
        try
        {
            var movement = await _context.Movements.FindAsync(id);
            if (movement == null) return false;

            movement.WarehouseId = movementDto.WarehouseId;
            movement.ProductId = movementDto.ProductId;
            movement.Description = movementDto.Description;
            movement.Quantity = movementDto.Quantity;
            movement.MovementDate = movementDto.MovementDate;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }

    }

    // Método para verificar si un Movement tiene registros hijos
    public async Task<bool> HasRelatedRecordsAsync(int movementId)
    {
        try
        {
            // Consulta optimizada - solo verifica existencia, no carga datos
            var hasBuy = await _context.Buys.AnyAsync(b => b.MovementId == movementId);
            var hasSell = await _context.Sells.AnyAsync(s => s.MovementId == movementId);
            var hasExpense = await _context.Expenses.AnyAsync(e => e.MovementId == movementId);

            return hasBuy || hasSell || hasExpense;
        }
        catch (Exception)
        {
            throw;
        }

    }

    // NUEVO: Método para verificar si existe un movimiento
    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _context.Movements.AnyAsync(m => m.Id == id);
        }
        catch (Exception)
        {
            throw;
        }

    }

    // NUEVO: Método para obtener movimientos por producto
    public async Task<IEnumerable<Movement>> GetByProductIdAsync(int productId)
    {
        try
        {
            return await _context.Movements
                .Where(m => m.ProductId == productId)
                .OrderByDescending(m => m.MovementDate)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }

    }

    // NUEVO: Método para obtener movimientos por rango de fechas
    public async Task<IEnumerable<Movement>> GetByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        try
        {
            // Validar que las fechas sean válidas
            if (startDate > endDate)
            {
                throw new ArgumentException("La fecha de inicio no puede ser mayor a la fecha de fin");
            }

            return await _context.Movements
                .Where(m => m.MovementDate >= startDate && m.MovementDate <= endDate)
                .OrderByDescending(m => m.MovementDate)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }

    }
}