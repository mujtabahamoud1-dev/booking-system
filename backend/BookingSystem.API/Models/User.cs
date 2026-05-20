namespace BookingSystem.API.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? Phone { get; set; }
    public string Role { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
