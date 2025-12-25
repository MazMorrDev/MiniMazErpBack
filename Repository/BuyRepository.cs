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
        // Consulta SQL correcta para la tabla Buy con su estructura real
        const string sql = """
            SELECT 
                movement_id,
                unit_price
            FROM "Buy"
            ORDER BY movement_id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        var result = new List<Buy>();
        using var reader = await command.ExecuteReaderAsync();

        // Leer cada fila y crear objetos Buy
        while (await reader.ReadAsync())
        {
            // Obtener el índice de las columnas una vez
            int movementIdOrdinal = reader.GetOrdinal("movement_id");
            int unitPriceOrdinal = reader.GetOrdinal("unit_price");

            var buy = new Buy
            {
                MovementId = reader.GetInt32(movementIdOrdinal),
                UnitPrice = reader.IsDBNull(unitPriceOrdinal) 
                    ? 0m  // Valor por defecto si es NULL
                    : reader.GetDecimal(unitPriceOrdinal),
                Movement = new Movement() // Objeto vacío pero no null
                {
                    // Inicializa las propiedades mínimas requeridas
                    Id = reader.GetInt32(movementIdOrdinal)
                }
            };

            result.Add(buy);
        }

        return result;
    }

    public async Task<Buy?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT 
                movement_id,
                unit_price
            FROM "Buy"
            WHERE movement_id = @id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            int movementIdOrdinal = reader.GetOrdinal("movement_id");
            int unitPriceOrdinal = reader.GetOrdinal("unit_price");

            return new Buy
            {
                MovementId = reader.GetInt32(movementIdOrdinal),
                UnitPrice = reader.IsDBNull(unitPriceOrdinal) 
                    ? 0m 
                    : reader.GetDecimal(unitPriceOrdinal),
                Movement = new Movement()
                {
                    Id = reader.GetInt32(movementIdOrdinal)
                }
            };
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
}
