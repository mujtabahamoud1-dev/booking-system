using Npgsql;

namespace BookingSystem.API.Shared.Database;

public class UnitOfWork : IUnitOfWork
{
    public NpgsqlConnection Connection { get; }
    public NpgsqlTransaction Transaction { get; }

    private bool _committed;

    private UnitOfWork(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        Connection  = connection;
        Transaction = transaction;
    }

    public static async Task<UnitOfWork> BeginAsync(DatabaseConnection db)
    {
        var connection  = await db.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        return new UnitOfWork(connection, transaction);
    }

    public async Task CommitAsync()
    {
        await Transaction.CommitAsync();
        _committed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_committed)
            await Transaction.RollbackAsync();

        await Transaction.DisposeAsync();
        await Connection.DisposeAsync();
    }
}
