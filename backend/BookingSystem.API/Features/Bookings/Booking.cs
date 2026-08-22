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

public class BookingWithSlot : Booking
{
    public TimeOnly SlotStartTime { get; set; }
    public TimeOnly SlotEndTime { get; set; }
}

public class BookingWithPatient : BookingWithSlot
{
    public string PatientName { get; set; } = default!;
    public string PatientEmail { get; set; } = default!;
    public string? PatientPhone { get; set; }
}
