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

// The admin queue's row: a booking plus who it belongs to. Kept separate from
// BookingResponse so the patient-facing endpoints cannot start returning contact
// details by accident — this shape is only ever served to an admin.
public record AdminBookingResponse(
    int Id,
    int UserId,
    int ServiceId,
    int SlotId,
    DateOnly BookingDate,
    BookingStatus Status,
    string? Notes,
    DateTime CreatedAt,
    string PatientName,
    string PatientEmail,
    string? PatientPhone)
{
    public static AdminBookingResponse From(BookingWithPatient b) =>
        new(b.Id, b.UserId, b.ServiceId, b.SlotId, b.BookingDate, b.Status, b.Notes, b.CreatedAt,
            b.PatientName, b.PatientEmail, b.PatientPhone);
}

// The admin queue's filters, as they arrive on the query string. Every one is
// optional; an empty query is "the whole queue".
public record AdminBookingQuery(
    string? Search = null,
    BookingStatus? Status = null,
    DateOnly? From = null,
    DateOnly? To = null);

// Counts let the queue say how many are pending without the admin having to
// filter to pending to find out. They honour the search and the date range but
// deliberately ignore the status filter — otherwise selecting "pending" would
// zero the other three and the row would stop being readable.
public record BookingStatusCounts(int Total, int Pending, int Confirmed, int Cancelled);

public record AdminBookingListResponse(List<AdminBookingResponse> Items, BookingStatusCounts Counts);

// The slot fields needed to validate a booking, read under a row lock.
public record SlotInfo(int ServiceId, int DayOfWeek, int MaxBookings);
