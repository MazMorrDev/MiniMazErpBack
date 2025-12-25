﻿using System.Data.Common;

namespace MiniMazErpBack;

public class SellRepository(Func<DbConnection> factory) : ISellRepository
{
    private readonly Func<DbConnection> _connectionFactory = factory;

    public async Task<int> CreateAsync(Sell sell)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO "Sell" (movement_id, sale_price, discount_percentage)
            VALUES (@movement_id, @sale_price, @discount_percentage)
            RETURNING movement_id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@movement_id", sell.MovementId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@sale_price", sell.SalePrice));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@discount_percentage", sell.DiscountPercentage));

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            DELETE FROM "Sell" 
            WHERE movement_id = @movement_id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@movement_id", id));

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<IEnumerable<Sell>> GetAllAsync()
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT s.movement_id, s.sale_price, s.discount_percentage,
                   m.warehouse_id, m.product_id, m.description, m.quantity, m.movement_date
            FROM "Sell" s
            INNER JOIN "Movement" m ON s.movement_id = m.id
            ORDER BY m.movement_date DESC, s.movement_id DESC;
            """;

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<Sell>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToSell(reader));
        }

        return result;
    }

    public async Task<Sell?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT s.movement_id, s.sale_price, s.discount_percentage,
                   m.warehouse_id, m.product_id, m.description, m.quantity, m.movement_date
            FROM "Sell" s
            INNER JOIN "Movement" m ON s.movement_id = m.id
            WHERE s.movement_id = @movement_id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@movement_id", id));

        using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return MapToSell(reader);
        }

        return null;
    }

    public async Task<bool> UpdateAsync(Sell sell)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE "Sell" 
            SET sale_price = @sale_price,
                discount_percentage = @discount_percentage
            WHERE movement_id = @movement_id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@sale_price", sell.SalePrice));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@discount_percentage", sell.DiscountPercentage));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@movement_id", sell.MovementId));

        return await command.ExecuteNonQueryAsync() > 0;
    }

    private static Sell MapToSell(DbDataReader reader)
    {
        var movementId = reader.GetInt32(reader.GetOrdinal("movement_id"));
        
        return new Sell
        {
            MovementId = movementId,
            SalePrice = reader.GetDecimal(reader.GetOrdinal("sale_price")),
            DiscountPercentage = reader.GetDecimal(reader.GetOrdinal("discount_percentage")),
            Movement = new Movement
            {
                Id = movementId,
                WarehouseId = reader.GetInt32(reader.GetOrdinal("warehouse_id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) 
                    ? null 
                    : reader.GetString(reader.GetOrdinal("description")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                MovementDate = reader.GetDateTime(reader.GetOrdinal("movement_date"))
            }
        };
    }

    // Método adicional útil para obtener ventas por producto
    public async Task<IEnumerable<Sell>> GetByProductIdAsync(int productId)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT s.movement_id, s.sale_price, s.discount_percentage,
                   m.warehouse_id, m.product_id, m.description, m.quantity, m.movement_date
            FROM "Sell" s
            INNER JOIN "Movement" m ON s.movement_id = m.id
            WHERE m.product_id = @product_id
            ORDER BY m.movement_date DESC;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@product_id", productId));

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<Sell>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToSell(reader));
        }

        return result;
    }

    // Método adicional útil para obtener ventas por rango de fechas
    public async Task<IEnumerable<Sell>> GetByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT s.movement_id, s.sale_price, s.discount_percentage,
                   m.warehouse_id, m.product_id, m.description, m.quantity, m.movement_date
            FROM "Sell" s
            INNER JOIN "Movement" m ON s.movement_id = m.id
            WHERE m.movement_date >= @start_date AND m.movement_date <= @end_date
            ORDER BY m.movement_date DESC;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@start_date", startDate));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@end_date", endDate));

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<Sell>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToSell(reader));
        }

        return result;
    }
}