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
                movement_id,
                expense_type,
                total_price
            FROM "Expense"
            ORDER BY movement_id;
            """;

        using var connection = _connectionFactory();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        var result = new List<Expense>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int movementIdOrdinal = reader.GetOrdinal("movement_id");
            int expenseTypeOrdinal = reader.GetOrdinal("expense_type");
            int totalPriceOrdinal = reader.GetOrdinal("total_price");

            var expense = new Expense
            {
                MovementId = reader.GetInt32(movementIdOrdinal),
                ExpenseType = (ExpenseType)reader.GetInt32(expenseTypeOrdinal),
                TotalPrice = reader.IsDBNull(totalPriceOrdinal)
                    ? 0m
                    : reader.GetDecimal(totalPriceOrdinal),
                Movement = new Movement()
                {
                    Id = reader.GetInt32(movementIdOrdinal)
                }
            };

            result.Add(expense);
        }

        return result;
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT 
                movement_id,
                expense_type,
                total_price
            FROM "Expense"
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
            int expenseTypeOrdinal = reader.GetOrdinal("expense_type");
            int totalPriceOrdinal = reader.GetOrdinal("total_price");

            return new Expense
            {
                MovementId = reader.GetInt32(movementIdOrdinal),
                ExpenseType = (ExpenseType)reader.GetInt32(expenseTypeOrdinal),
                TotalPrice = reader.IsDBNull(totalPriceOrdinal)
                    ? 0m
                    : reader.GetDecimal(totalPriceOrdinal),
                Movement = new Movement()
                {
                    Id = reader.GetInt32(movementIdOrdinal)
                }
            };
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
}