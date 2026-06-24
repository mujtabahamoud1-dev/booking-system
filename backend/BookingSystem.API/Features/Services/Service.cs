namespace BookingSystem.API.Features.Services;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
