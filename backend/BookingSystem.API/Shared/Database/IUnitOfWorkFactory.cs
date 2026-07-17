namespace BookingSystem.API.Shared.Database;

public interface IUnitOfWorkFactory
{
    Task<T> ExecuteAsync<T>(Func<IUnitOfWork, Task<T>> work);
}
