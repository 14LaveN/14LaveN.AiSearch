using System.Data;
using Npgsql;

namespace Persistence;

/// <summary>
/// Represents the db connection static class.
/// </summary>
public static class DbConnection
{
    private const string? ConnectionString = "Server=localhost;Port=5433;Database=PPGenericDb;User Id=postgres;Password=1111;";

    /// <summary>
    /// Create db connection with specified connection string.
    /// </summary>
    /// <returns>Returns db connection.</returns>
    public static IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(ConnectionString);
    }
}