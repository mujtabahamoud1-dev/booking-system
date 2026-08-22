namespace BookingSystem.API.Features.Services;

public interface IServiceRepository
{
    Task<List<ServiceWithDays>> GetAllAsync(bool includeInactive);
    Task<List<ServiceWithDays>> SearchAsync(ServiceQuery query);
    Task<ServiceCounts> CountAsync(ServiceQuery query);
    Task<ServiceWithDays?> GetByIdAsync(int id);
    Task<ServiceWithDays> CreateAsync(Service service);
    Task<ServiceWithDays?> UpdateAsync(Service service);
    Task<bool> DeleteAsync(int id);
}
