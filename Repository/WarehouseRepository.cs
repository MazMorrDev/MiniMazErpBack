﻿using System.Data.Common;

namespace MiniMazErpBack;

public class WarehouseRepository(Func<DbConnection> factory) : IWarehouseRepository
{
    private readonly Func<DbConnection> _connectionFactory = factory;

    public async Task<int> CreateAsync(Warehouse warehouse)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO "Warehouse" (name, description)
            VALUES (@name, @description)
            RETURNING id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@name", warehouse.Name));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@description", warehouse.Description));

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            DELETE FROM "Warehouse" 
            WHERE id = @id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync()
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, description
            FROM "Warehouse"
            ORDER BY name;
            """;

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<Warehouse>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToWarehouse(reader));
        }

        return result;
    }

    public async Task<Warehouse?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, description
            FROM "Warehouse"
            WHERE id = @id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return MapToWarehouse(reader);
        }

        return null;
    }

    public async Task<bool> UpdateAsync(Warehouse warehouse)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE "Warehouse" 
            SET name = @name,
                description = @description
            WHERE id = @id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@name", warehouse.Name));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@description", warehouse.Description));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", warehouse.Id));

        return await command.ExecuteNonQueryAsync() > 0;
    }

    private static Warehouse MapToWarehouse(DbDataReader reader)
    {
        return new Warehouse
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Description = reader.IsDBNull(reader.GetOrdinal("description")) 
                ? null 
                : reader.GetString(reader.GetOrdinal("description"))
        };
    }

    // Método adicional útil para buscar por nombre
    public async Task<Warehouse?> GetByNameAsync(string name)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, description
            FROM "Warehouse"
            WHERE name = @name;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@name", name));

        using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return MapToWarehouse(reader);
        }

        return null;
    }

    // Método adicional útil para buscar por coincidencia de nombre
    public async Task<IEnumerable<Warehouse>> SearchByNameAsync(string searchTerm)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, description
            FROM "Warehouse"
            WHERE name ILIKE @search_term
            ORDER BY name;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@search_term", $"%{searchTerm}%"));

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<Warehouse>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToWarehouse(reader));
        }

        return result;
    }
}