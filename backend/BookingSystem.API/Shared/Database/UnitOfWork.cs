namespace BookingSystem.API.Shared.Database;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DbSession _session;

    public UnitOfWork(DbSession session) => _session = session;

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> work)
    {
        await using var tx = await _session.BeginTransactionAsync();
        try
        {
            var result = await work();
            await tx.CommitAsync();
            return result;
        }
        finally
        {
            // Disposal of an uncommitted transaction rolls back; clear it either way.
            _session.ClearTransaction();
        }
    }
}
