using System.Data.Common;

namespace MiniMazErpBack;

public class ExpenseRepository(Func<DbConnection> factory) : IExpenseRepository
{
    private readonly Func<DbConnection> _connectionFactory = factory;

    public async Task<int> CreateAsync(Expense expense)
    {
        const string sql = """
            INSERT INTO "Expense" (movement_id, expense_type, total_price)
            VALUES (@movement_id, @expense_type, @total_price)
            RETURNING movement_id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@movement_id", expense.MovementId));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@expense_type", expense.ExpenseType));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@total_price", expense.TotalPrice));

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = """
            DELETE FROM "Expense" 
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

    public async Task<IEnumerable<Expense>> GetAllAsync()
    {
        const string sql = """
            SELECT 
                e.movement_id,
                e.expense_type,
                e.total_price,
                m.warehouse_id,
                m.product_id,
                m.description,
                m.quantity,
                m.movement_date
            FROM "Expense" e
            INNER JOIN "Movement" m ON e.movement_id = m.id
            ORDER BY m.movement_date DESC, e.movement_id DESC;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        var result = new List<Expense>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(MapToExpense(reader));
        }

        return result;
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT 
                e.movement_id,
                e.expense_type,
                e.total_price,
                m.warehouse_id,
                m.product_id,
                m.description,
                m.quantity,
                m.movement_date
            FROM "Expense" e
            INNER JOIN "Movement" m ON e.movement_id = m.id
            WHERE e.movement_id = @id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@id", id));

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapToExpense(reader);
        }

        return null;
    }

    public async Task<bool> UpdateAsync(Expense expense)
    {
        const string sql = """
            UPDATE "Expense" 
            SET expense_type = @expense_type,
                total_price = @total_price
            WHERE movement_id = @movement_id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@expense_type", expense.ExpenseType));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@total_price", expense.TotalPrice));
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@movement_id", expense.MovementId));

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    // Nuevo método: Verificar si existe un registro Expense
    public async Task<bool> ExistsAsync(int id)
    {
        const string sql = """
            SELECT COUNT(1) 
            FROM "Expense" 
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

    // Nuevo método: Obtener gastos por tipo
    public async Task<IEnumerable<Expense>> GetByTypeAsync(ExpenseType expenseType)
    {
        const string sql = """
            SELECT 
                e.movement_id,
                e.expense_type,
                e.total_price,
                m.warehouse_id,
                m.product_id,
                m.description,
                m.quantity,
                m.movement_date
            FROM "Expense" e
            INNER JOIN "Movement" m ON e.movement_id = m.id
            WHERE e.expense_type = @expense_type
            ORDER BY m.movement_date DESC;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(DbParameterHelper.CreateParameter(command, "@expense_type", expenseType));

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<Expense>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToExpense(reader));
        }

        return result;
    }

    // Nuevo método: Obtener gastos por rango de fechas
    public async Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        const string sql = """
            SELECT 
                e.movement_id,
                e.expense_type,
                e.total_price,
                m.warehouse_id,
                m.product_id,
                m.description,
                m.quantity,
                m.movement_date
            FROM "Expense" e
            INNER JOIN "Movement" m ON e.movement_id = m.id
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
        var result = new List<Expense>();

        while (await reader.ReadAsync())
        {
            result.Add(MapToExpense(reader));
        }

        return result;
    }

    private static Expense MapToExpense(DbDataReader reader)
    {
        var movementId = reader.GetInt32(reader.GetOrdinal("movement_id"));

        return new Expense
        {
            MovementId = movementId,
            ExpenseType = (ExpenseType)reader.GetInt32(reader.GetOrdinal("expense_type")),
            TotalPrice = reader.GetDecimal(reader.GetOrdinal("total_price")),
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