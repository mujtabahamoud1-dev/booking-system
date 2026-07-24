using Npgsql;

namespace BookingSystem.API.Features.Services;

public class ServiceService : IServiceService
{
    // PostgreSQL SQLSTATE for a foreign_key_violation.
    private const string ForeignKeyViolation = "23503";

    private readonly IServiceRepository _services;

    public ServiceService(IServiceRepository services) => _services = services;

    public async Task<List<ServiceResponse>> GetAllAsync(bool includeInactive)
    {
        var services = await _services.GetAllAsync(includeInactive);
        return services.Select(ServiceResponse.From).ToList();
    }

    public async Task<ServiceResponse?> GetByIdAsync(int id)
    {
        var service = await _services.GetByIdAsync(id);
        return service is null ? null : ServiceResponse.From(service);
    }

    public async Task<ServiceResponse> CreateAsync(CreateServiceRequest req)
    {
        var service = await _services.CreateAsync(new Service
        {
            Name        = req.Name,
            Description = req.Description,
            Duration    = req.Duration,
            Price       = req.Price
        });
        return ServiceResponse.From(service);
    }

    public async Task<ServiceResponse?> UpdateAsync(int id, UpdateServiceRequest req)
    {
        var service = await _services.UpdateAsync(new Service
        {
            Id          = id,
            Name        = req.Name,
            Description = req.Description,
            Duration    = req.Duration,
            Price       = req.Price,
            IsActive    = req.IsActive
        });
        return service is null ? null : ServiceResponse.From(service);
    }

    public async Task<DeleteServiceResult> DeleteAsync(int id)
    {
        try
        {
            return await _services.DeleteAsync(id)
                ? DeleteServiceResult.Deleted
                : DeleteServiceResult.NotFound;
        }
        catch (PostgresException ex) when (ex.SqlState == ForeignKeyViolation)
        {
            // The service is referenced by existing bookings; deactivate it instead.
            return DeleteServiceResult.InUse;
        }
    }
}
