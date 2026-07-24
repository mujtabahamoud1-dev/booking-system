namespace BookingSystem.API.Features.Bookings;

public record CreateBookingRequest(int ServiceId, int SlotId, DateOnly BookingDate, string? Notes);

public record BookingResponse(
    int Id,
    int UserId,
    int ServiceId,
    int SlotId,
    DateOnly BookingDate,
    BookingStatus Status,
    string? Notes,
    DateTime CreatedAt)
{
    public static BookingResponse From(Booking b) =>
        new(b.Id, b.UserId, b.ServiceId, b.SlotId, b.BookingDate, b.Status, b.Notes, b.CreatedAt);
}

// The slot fields needed to validate a booking, read under a row lock.
public record SlotInfo(int ServiceId, int DayOfWeek, int MaxBookings);
