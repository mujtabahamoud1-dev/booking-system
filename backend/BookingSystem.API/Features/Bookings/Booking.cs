namespace BookingSystem.API.Features.Bookings;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ServiceId { get; set; }
    public int SlotId { get; set; }
    public DateOnly BookingDate { get; set; }
    public BookingStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

// A booking joined with the patient who made it. Only the admin queue reads this:
// staff work the queue by name — someone phones about "Thursday" and a bare user
// id is not something you can search. A patient reading their own bookings
// already knows who they are, so that path keeps the plain Booking.
public class BookingWithPatient : Booking
{
    public string PatientName { get; set; } = default!;
    public string PatientEmail { get; set; } = default!;
    public string? PatientPhone { get; set; }
}
