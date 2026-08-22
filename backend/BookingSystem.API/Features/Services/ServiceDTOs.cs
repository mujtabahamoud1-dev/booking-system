namespace BookingSystem.API.Features.Services;

public record CreateServiceRequest(
    string Name,
    string? NameAr,
    string? Description,
    string? DescriptionAr,
    int Duration,
    decimal Price);

public record UpdateServiceRequest(
    string Name,
    string? NameAr,
    string? Description,
    string? DescriptionAr,
    int Duration,
    decimal Price,
    bool IsActive);

public record ServiceResponse(
    int Id,
    string Name,
    string? NameAr,
    string? Description,
    string? DescriptionAr,
    int Duration,
    decimal Price,
    bool IsActive,
    int[] Days)
{
    public static ServiceResponse From(ServiceWithDays s) =>
        new(s.Id, s.Name, s.NameAr, s.Description, s.DescriptionAr, s.Duration, s.Price,
            s.IsActive, s.Days);
}

public record ServiceQuery(string? Search = null, bool? IsActive = null);

public record ServiceCounts(int Total, int Active, int Inactive);

public record ServiceListResponse(List<ServiceResponse> Items, ServiceCounts Counts);
