namespace BookingSystem.API.Features.Services;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }

    public int Duration { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}

public class ServiceWithDays : Service
{
    public int[] Days { get; set; } = [];
}
