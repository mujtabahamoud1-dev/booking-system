namespace BookingSystem.API.Features.Bookings;

public record CreateBookingRequest(int ServiceId, int SlotId, DateOnly BookingDate, string? Notes);

public record BookingResponse(
    int Id,
    int UserId,
    int ServiceId,
    int SlotId,
    DateOnly BookingDate,
    TimeOnly SlotStartTime,
    TimeOnly SlotEndTime,
    BookingStatus Status,
    string? Notes,
    DateTime CreatedAt)
{
    public static BookingResponse From(BookingWithSlot b) =>
        new(b.Id, b.UserId, b.ServiceId, b.SlotId, b.BookingDate,
            b.SlotStartTime, b.SlotEndTime, b.Status, b.Notes, b.CreatedAt);
}

public record AdminBookingResponse(
    int Id,
    int UserId,
    int ServiceId,
    int SlotId,
    DateOnly BookingDate,
    TimeOnly SlotStartTime,
    TimeOnly SlotEndTime,
    BookingStatus Status,
    string? Notes,
    DateTime CreatedAt,
    string PatientName,
    string PatientEmail,
    string? PatientPhone)
{
    public static AdminBookingResponse From(BookingWithPatient b) =>
        new(b.Id, b.UserId, b.ServiceId, b.SlotId, b.BookingDate,
            b.SlotStartTime, b.SlotEndTime, b.Status, b.Notes, b.CreatedAt,
            b.PatientName, b.PatientEmail, b.PatientPhone);
}

// The admin queue's filters, as they arrive on the query string. Every one is
// optional; an empty query is "the whole queue".
public record AdminBookingQuery(
    string? Search = null,
    BookingStatus? Status = null,
    DateOnly? From = null,
    DateOnly? To = null);

public record BookingStatusCounts(int Total, int Pending, int Confirmed, int Cancelled);

public record AdminBookingListResponse(List<AdminBookingResponse> Items, BookingStatusCounts Counts);
public record SlotInfo(int ServiceId, int DayOfWeek, int MaxBookings);
