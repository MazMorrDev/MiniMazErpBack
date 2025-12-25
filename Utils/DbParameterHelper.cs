using System.Data.Common;

namespace MiniMazErpBack;

public class DbParameterHelper
{
    // Método helper para crear parámetros de forma segura
    public static DbParameter CreateParameter(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        return parameter;
    }
}
