namespace BookingSystem.API.Features.Services;

public interface IServiceRepository
{
    Task<List<Service>> GetAllAsync(bool includeInactive);
    Task<Service?> GetByIdAsync(int id);
    Task<Service> CreateAsync(string name, string? description, int duration, decimal price);
    Task<Service?> UpdateAsync(int id, string name, string? description, int duration, decimal price, bool isActive);
    Task<bool> DeleteAsync(int id);
}
