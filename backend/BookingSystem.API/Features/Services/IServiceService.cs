namespace BookingSystem.API.Features.Services;

public enum DeleteServiceResult
{
    Deleted,
    NotFound,
    InUse
}

public interface IServiceService
{
    Task<List<ServiceResponse>> GetAllAsync(bool includeInactive);
    Task<ServiceListResponse> SearchAsync(ServiceQuery query);
    Task<ServiceResponse?> GetByIdAsync(int id);
    Task<ServiceResponse> CreateAsync(CreateServiceRequest req);
    Task<ServiceResponse?> UpdateAsync(int id, UpdateServiceRequest req);
    Task<DeleteServiceResult> DeleteAsync(int id);
}
