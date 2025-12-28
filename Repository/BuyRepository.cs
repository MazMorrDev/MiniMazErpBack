using System.Data.Common;

namespace MiniMazErpBack;

public class BuyRepository(Func<DbConnection> factory) : IBuyRepository
{
    private readonly Func<DbConnection> _connectionFactory = factory;

    public async Task<int> CreateAsync(Buy buy)
    {
        const string sql = """
            INSERT INTO "Buy" (movement_id, unit_price)
            VALUES (@movement_id, @unit_price)
            RETURNING movement_id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@movement_id", buy.MovementId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@unit_price", buy.UnitPrice));

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = """
            DELETE FROM "Buy" 
            WHERE movement_id = @id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Buy>> GetAllAsync()
    {
        const string sql = """
            SELECT 
                b.movement_id,
                b.unit_price,
                m.warehouse_id,
                m.product_id,
                m.description,
                m.quantity,
                m.movement_date
            FROM "Buy" b
            INNER JOIN "Movement" m ON b.movement_id = m.id
            ORDER BY m.movement_date DESC, b.movement_id DESC;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        var result = new List<Buy>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(MapToBuy(reader));
        }

        return result;
    }

    public async Task<Buy?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT 
                b.movement_id,
                b.unit_price,
                m.warehouse_id,
                m.product_id,
                m.description,
                m.quantity,
                m.movement_date
            FROM "Buy" b
            INNER JOIN "Movement" m ON b.movement_id = m.id
            WHERE b.movement_id = @id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapToBuy(reader);
        }

        return null;
    }

    public async Task<bool> UpdateAsync(Buy buy)
    {
        const string sql = """
            UPDATE "Buy" 
            SET unit_price = @unit_price
            WHERE movement_id = @movement_id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@unit_price", buy.UnitPrice));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@movement_id", buy.MovementId));

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    // Nuevo método: Verificar si existe un registro Buy
    public async Task<bool> ExistsAsync(int id)
    {
        const string sql = """
            SELECT COUNT(1) 
            FROM "Buy" 
            WHERE movement_id = @id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }

    // Nuevo método: Obtener compras por producto
    public async Task<IEnumerable<Buy>> GetByProductIdAsync(int productId)
    {
        const string sql = """
            SELECT 
                b.movement_id,
                b.unit_price,
                m.warehouse_id,
                m.product_id,
                m.description,
                m.quantity,
                m.movement_date
            FROM "Buy" b
            INNER JOIN "Movement" m ON b.movement_id = m.id
            WHERE m.product_id = @product_id
            ORDER BY m.movement_date DESC;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@product_id", productId));

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<Buy>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToBuy(reader));
        }

        return result;
    }

    // Nuevo método: Obtener compras por rango de fechas
    public async Task<IEnumerable<Buy>> GetByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        const string sql = """
            SELECT 
                b.movement_id,
                b.unit_price,
                m.warehouse_id,
                m.product_id,
                m.description,
                m.quantity,
                m.movement_date
            FROM "Buy" b
            INNER JOIN "Movement" m ON b.movement_id = m.id
            WHERE m.movement_date >= @start_date AND m.movement_date <= @end_date
            ORDER BY m.movement_date DESC;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@start_date", startDate));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@end_date", endDate));

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<Buy>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToBuy(reader));
        }

        return result;
    }

    private static Buy MapToBuy(DbDataReader reader)
    {
        var movementId = reader.GetInt32(reader.GetOrdinal("movement_id"));

        return new Buy
        {
            MovementId = movementId,
            UnitPrice = reader.GetDecimal(reader.GetOrdinal("unit_price")),
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
}