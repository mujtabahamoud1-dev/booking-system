namespace BookingSystem.API.Features.Bookings;

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled
}

public static class BookingStatusExtensions
{
    // The bookings.status column is a VARCHAR guarded by the chk_status CHECK constraint,
    // so the enum is written out as the exact lowercase value the constraint allows.
    // (Dapper parses the column back into the enum case-insensitively on the way in.)
    public static string ToDbValue(this BookingStatus status) => status switch
    {
        BookingStatus.Pending   => "pending",
        BookingStatus.Confirmed => "confirmed",
        BookingStatus.Cancelled => "cancelled",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown booking status.")
    };
}
