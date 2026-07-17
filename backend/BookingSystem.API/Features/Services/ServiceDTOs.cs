namespace BookingSystem.API.Features.Services;

public record CreateServiceRequest(string Name, string? Description, int Duration, decimal Price);

public record UpdateServiceRequest(string Name, string? Description, int Duration, decimal Price, bool IsActive);

public record ServiceResponse(int Id, string Name, string? Description, int Duration, decimal Price, bool IsActive)
{
    public static ServiceResponse From(Service s) =>
        new(s.Id, s.Name, s.Description, s.Duration, s.Price, s.IsActive);
}
