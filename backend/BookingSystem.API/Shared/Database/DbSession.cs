using Npgsql;

namespace BookingSystem.API.Shared.Database;

/// <summary>
/// A single database connection per request (registered scoped). All repositories
/// share it, so a transaction opened here is automatically used by every command on
/// the same connection — Npgsql enlists commands in the connection's active
/// transaction — and nothing needs to be threaded through repository methods.
/// </summary>
public sealed class DbSession : IAsyncDisposable
{
    private readonly DatabaseConnection _db;
    private NpgsqlConnection? _connection;

    public DbSession(DatabaseConnection db) => _db = db;

    /// <summary>The transaction currently active on this session's connection, if any.</summary>
    public NpgsqlTransaction? Transaction { get; private set; }

    /// <summary>Returns the shared connection, opening it lazily on first use.</summary>
    public async Task<NpgsqlConnection> GetConnectionAsync()
    {
        if (_connection is null)
            _connection = await _db.OpenAsync();
        return _connection;
    }

    public async Task<NpgsqlTransaction> BeginTransactionAsync()
    {
        if (Transaction is not null)
            throw new InvalidOperationException("A transaction is already active for this session.");

        var conn = await GetConnectionAsync();
        Transaction = await conn.BeginTransactionAsync();
        return Transaction;
    }

    internal void ClearTransaction() => Transaction = null;

    public async ValueTask DisposeAsync()
    {
        if (Transaction is not null)
            await Transaction.DisposeAsync();
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
