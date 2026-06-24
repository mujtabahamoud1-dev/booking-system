namespace BookingSystem.API.Features.Bookings;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ServiceId { get; set; }
    public int SlotId { get; set; }
    public DateOnly BookingDate { get; set; }
    public string Status { get; set; } = default!;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
