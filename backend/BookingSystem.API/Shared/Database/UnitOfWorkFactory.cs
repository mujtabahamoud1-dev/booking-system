namespace BookingSystem.API.Shared.Database;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly DatabaseConnection _db;

    public UnitOfWorkFactory(DatabaseConnection db) => _db = db;

    public async Task<T> ExecuteAsync<T>(Func<IUnitOfWork, Task<T>> work)
    {
        await using var uow = await UnitOfWork.BeginAsync(_db);
        var result = await work(uow);
        await uow.CommitAsync();
        return result;
    }
}
