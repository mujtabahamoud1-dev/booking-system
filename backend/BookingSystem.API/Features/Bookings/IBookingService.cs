namespace BookingSystem.API.Features.Bookings;

public enum ChangeStatus
{
    Updated,
    NotFound,
    Forbidden
}

public class ChangeStatusResult
{
    public ChangeStatus Status { get; }
    public BookingResponse? Booking { get; }

    private ChangeStatusResult(ChangeStatus status, BookingResponse? booking)
    {
        Status  = status;
        Booking = booking;
    }

    public static ChangeStatusResult Updated(BookingResponse booking) => new(ChangeStatus.Updated, booking);
    public static ChangeStatusResult NotFound() => new(ChangeStatus.NotFound, null);
    public static ChangeStatusResult Forbidden() => new(ChangeStatus.Forbidden, null);
}

public interface IBookingService
{
    Task<CreateBookingResult> CreateAsync(int userId, CreateBookingRequest req);
    Task<List<BookingResponse>> GetForUserAsync(int userId);
    Task<AdminBookingListResponse> GetAllAsync(AdminBookingQuery query);
    Task<ChangeStatusResult> CancelAsync(int id, int requestingUserId, bool isAdmin);
    Task<ChangeStatusResult> ConfirmAsync(int id);
}
