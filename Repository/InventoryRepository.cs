using System.Data.Common;

namespace MiniMazErpBack;

public class InventoryRepository(Func<DbConnection> factory) : IInventoryRepository
{
    private readonly Func<DbConnection> _connectionFactory = factory;

    public async Task<int> CreateAsync(Inventory inventory)
    {
        const string sql = """
            INSERT INTO "Inventory" (warehouse_id, product_id, stock, alert_stock, warning_stock)
            VALUES (@warehouse_id, @product_id, @stock, @alert_stock, @warning_stock)
            RETURNING id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@warehouse_id", inventory.WarehouseId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@product_id", inventory.ProductId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@stock", inventory.Stock));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@alert_stock", inventory.AlertStock));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@warning_stock", inventory.WarningStock));

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = """
            DELETE FROM "Inventory" 
            WHERE id = @id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Inventory>> GetAllAsync()
    {
        const string sql = """
            SELECT 
                id,
                warehouse_id,
                product_id,
                stock,
                alert_stock,
                warning_stock
            FROM "Inventory"
            ORDER BY id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        var result = new List<Inventory>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int idOrdinal = reader.GetOrdinal("id");
            int warehouseIdOrdinal = reader.GetOrdinal("warehouse_id");
            int productIdOrdinal = reader.GetOrdinal("product_id");
            int stockOrdinal = reader.GetOrdinal("stock");
            int alertStockOrdinal = reader.GetOrdinal("alert_stock");
            int warningStockOrdinal = reader.GetOrdinal("warning_stock");

            var inventory = new Inventory
            {
                Id = reader.GetInt32(idOrdinal),
                WarehouseId = reader.GetInt32(warehouseIdOrdinal),
                ProductId = reader.GetInt32(productIdOrdinal),
                Stock = reader.GetInt32(stockOrdinal),
                AlertStock = reader.IsDBNull(alertStockOrdinal) 
                    ? null 
                    : reader.GetInt32(alertStockOrdinal),
                WarningStock = reader.IsDBNull(warningStockOrdinal) 
                    ? null 
                    : reader.GetInt32(warningStockOrdinal),
                Warehouse = new Warehouse()
                {
                    Id = reader.GetInt32(warehouseIdOrdinal)
                },
                Product = new Product()
                {
                    Id = reader.GetInt32(productIdOrdinal)
                }
            };

            result.Add(inventory);
        }

        return result;
    }

    public async Task<Inventory?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT 
                id,
                warehouse_id,
                product_id,
                stock,
                alert_stock,
                warning_stock
            FROM "Inventory"
            WHERE id = @id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            int idOrdinal = reader.GetOrdinal("id");
            int warehouseIdOrdinal = reader.GetOrdinal("warehouse_id");
            int productIdOrdinal = reader.GetOrdinal("product_id");
            int stockOrdinal = reader.GetOrdinal("stock");
            int alertStockOrdinal = reader.GetOrdinal("alert_stock");
            int warningStockOrdinal = reader.GetOrdinal("warning_stock");

            return new Inventory
            {
                Id = reader.GetInt32(idOrdinal),
                WarehouseId = reader.GetInt32(warehouseIdOrdinal),
                ProductId = reader.GetInt32(productIdOrdinal),
                Stock = reader.GetInt32(stockOrdinal),
                AlertStock = reader.IsDBNull(alertStockOrdinal) 
                    ? null 
                    : reader.GetInt32(alertStockOrdinal),
                WarningStock = reader.IsDBNull(warningStockOrdinal) 
                    ? null 
                    : reader.GetInt32(warningStockOrdinal),
                Warehouse = new Warehouse()
                {
                    Id = reader.GetInt32(warehouseIdOrdinal)
                },
                Product = new Product()
                {
                    Id = reader.GetInt32(productIdOrdinal)
                }
            };
        }

        return null;
    }

    public async Task<bool> UpdateAsync(Inventory inventory)
    {
        const string sql = """
            UPDATE "Inventory" 
            SET warehouse_id = @warehouse_id,
                product_id = @product_id,
                stock = @stock,
                alert_stock = @alert_stock,
                warning_stock = @warning_stock
            WHERE id = @id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@warehouse_id", inventory.WarehouseId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@product_id", inventory.ProductId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@stock", inventory.Stock));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@alert_stock", inventory.AlertStock));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@warning_stock", inventory.WarningStock));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", inventory.Id));

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}