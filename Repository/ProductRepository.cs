﻿using System.Data.Common;

namespace MiniMazErpBack;

public class ProductRepository(Func<DbConnection> factory) : IProductRepository
{
    private readonly Func<DbConnection> _connectionFactory = factory;

    public async Task<int> CreateAsync(Product product)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO "Product" (name, sell_price)
            VALUES (@name, @sell_price)
            RETURNING id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@name", product.Name));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@sell_price", product.SellPrice));

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            DELETE FROM "Product" 
            WHERE id = @id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, sell_price
            FROM "Product"
            ORDER BY name;
            """;

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<Product>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToProduct(reader));
        }

        return result;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, sell_price
            FROM "Product"
            WHERE id = @id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return MapToProduct(reader);
        }

        return null;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE "Product" 
            SET name = @name,
                sell_price = @sell_price
            WHERE id = @id;
            """;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@name", product.Name));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@sell_price", product.SellPrice));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", product.Id));

        return await command.ExecuteNonQueryAsync() > 0;
    }

    private static Product MapToProduct(DbDataReader reader)
    {
        return new Product
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            SellPrice = reader.IsDBNull(reader.GetOrdinal("sell_price")) 
                ? null 
                : reader.GetDecimal(reader.GetOrdinal("sell_price"))
        };
    }
}