using System.Data.Common;
using DotNetEnv;
using Npgsql;

namespace MiniMazErpBack;

public static class DatabaseConfig
{
    
    // Cadena de conexión CORRECTA para PostgreSQL
    public static string ConnectionString { get; } =
        Env.GetString("DATABASE_CONNECTION");

    public static Func<DbConnection> ConnectionFactory => () =>
    {
        var connection = new NpgsqlConnection(ConnectionString);
        return connection;
    };
}

// // How To Use DatabaseConfig:
//    private static readonly Func<DbConnection> _connectionFactory = DatabaseConfig.ConnectionFactory;
//    private static readonly UserService _userService = new(_connectionFactory);
