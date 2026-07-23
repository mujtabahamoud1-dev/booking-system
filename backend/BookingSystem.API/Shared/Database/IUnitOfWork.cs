namespace BookingSystem.API.Shared.Database;

public interface IUnitOfWork
{
    /// <summary>
    /// Runs <paramref name="work"/> inside a transaction on the request's shared
    /// connection, committing on success and rolling back if it throws. Repositories
    /// called within <paramref name="work"/> automatically take part — nothing is passed in.
    /// </summary>
    Task<T> ExecuteAsync<T>(Func<Task<T>> work);
}
