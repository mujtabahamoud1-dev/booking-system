using System.Reflection;
using DbUp;

namespace BookingSystem.API.Shared.Database;

/// <summary>
/// Applies pending database migrations on startup. Scripts live in the
/// Migrations/ folder as embedded resources and run once each, in filename
/// order. DbUp records applied scripts in a "schemaversions" table.
/// </summary>
public static class DatabaseMigrator
{
    public static void Run(string connectionString)
    {
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransactionPerScript()
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
            throw new Exception("Database migration failed.", result.Error);
    }
}
