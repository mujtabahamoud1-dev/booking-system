namespace BookingSystem.API.Features.Services;

public record CreateServiceRequest(string Name, string? Description, int Duration, decimal Price);

public record UpdateServiceRequest(string Name, string? Description, int Duration, decimal Price, bool IsActive);

public record ServiceResponse(int Id, string Name, string? Description, int Duration, decimal Price, bool IsActive)
{
    public static ServiceResponse From(Service s) =>
        new(s.Id, s.Name, s.Description, s.Duration, s.Price, s.IsActive);
}

// The admin list's filters. `IsActive` null means "both"; a service with bookings
// can only be deactivated, never deleted, so retired ones accumulate and the
// admin needs to be able to put them out of view.
public record ServiceQuery(string? Search = null, bool? IsActive = null);

// Honours the search but not the active filter, so the chips can show how many
// are hiding on the other side of it.
public record ServiceCounts(int Total, int Active, int Inactive);

public record ServiceListResponse(List<ServiceResponse> Items, ServiceCounts Counts);
