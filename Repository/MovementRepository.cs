﻿using System.Data.Common;

namespace MiniMazErpBack;

public class MovementRepository(Func<DbConnection> factory) : IMovementRepository
{
    private readonly Func<DbConnection> _connectionFactory = factory;

    public async Task<int> CreateAsync(Movement movement)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO "Movement" (warehouse_id, product_id, description, quantity, movement_date)
            VALUES (@warehouse_id, @product_id, @description, @quantity, @movement_date)
            RETURNING id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@warehouse_id", movement.WarehouseId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@product_id", movement.ProductId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@description", movement.Description));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@quantity", movement.Quantity));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@movement_date", movement.MovementDate));

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            DELETE FROM "Movement" 
            WHERE id = @id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<IEnumerable<Movement>> GetAllAsync()
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, warehouse_id, product_id, description, quantity, movement_date
            FROM "Movement"
            ORDER BY movement_date DESC, id DESC;
            """;

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<Movement>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToMovement(reader));
        }

        return result;
    }

    public async Task<Movement?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, warehouse_id, product_id, description, quantity, movement_date
            FROM "Movement"
            WHERE id = @id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return MapToMovement(reader);
        }

        return null;
    }

    public async Task<bool> UpdateAsync(Movement movement)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE "Movement" 
            SET warehouse_id = @warehouse_id,
                product_id = @product_id,
                description = @description,
                quantity = @quantity,
                movement_date = @movement_date
            WHERE id = @id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@warehouse_id", movement.WarehouseId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@product_id", movement.ProductId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@description", movement.Description));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@quantity", movement.Quantity));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@movement_date", movement.MovementDate));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", movement.Id));

        return await command.ExecuteNonQueryAsync() > 0;
    }

    private static Movement MapToMovement(DbDataReader reader)
    {
        return new Movement
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            WarehouseId = reader.GetInt32(reader.GetOrdinal("warehouse_id")),
            ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
            Description = reader.IsDBNull(reader.GetOrdinal("description")) 
                ? null 
                : reader.GetString(reader.GetOrdinal("description")),
            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
            MovementDate = reader.GetDateTime(reader.GetOrdinal("movement_date"))
        };
    }
}