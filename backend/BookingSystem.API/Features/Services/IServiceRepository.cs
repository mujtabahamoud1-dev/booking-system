namespace BookingSystem.API.Features.Services;

public interface IServiceRepository
{
    Task<List<Service>> GetAllAsync(bool includeInactive);
    Task<Service?> GetByIdAsync(int id);
    Task<Service> CreateAsync(Service service);
    Task<Service?> UpdateAsync(Service service);
    Task<bool> DeleteAsync(int id);
}
