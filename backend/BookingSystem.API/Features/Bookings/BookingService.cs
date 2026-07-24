using BookingSystem.API.Shared.Database;

namespace BookingSystem.API.Features.Bookings;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IBookingRepository bookings, IUnitOfWork unitOfWork)
    {
        _bookings   = bookings;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateBookingResult> CreateAsync(int userId, CreateBookingRequest req)
    {
        if (req.BookingDate < DateOnly.FromDateTime(DateTime.UtcNow))
            return CreateBookingResult.Fail(CreateBookingFailure.DateInPast);

        // Lock the slot, verify it, check capacity, and insert — all in one transaction so
        // two concurrent requests can't both slip past a not-yet-full check on the same slot.
        return await _unitOfWork.ExecuteAsync(async () =>
        {
            var slot = await _bookings.LockSlotAsync(req.SlotId);
            if (slot is null)
                return CreateBookingResult.Fail(CreateBookingFailure.SlotNotFound);
            if (slot.ServiceId != req.ServiceId)
                return CreateBookingResult.Fail(CreateBookingFailure.ServiceMismatch);
            if (slot.DayOfWeek != (int)req.BookingDate.DayOfWeek)
                return CreateBookingResult.Fail(CreateBookingFailure.DayMismatch);

            var taken = await _bookings.CountActiveForSlotAsync(req.SlotId, req.BookingDate);
            if (taken >= slot.MaxBookings)
                return CreateBookingResult.Fail(CreateBookingFailure.SlotFull);

            var booking = await _bookings.CreateAsync(userId, req.ServiceId, req.SlotId, req.BookingDate, req.Notes);
            return CreateBookingResult.Success(BookingResponse.From(booking));
        });
    }

    public async Task<List<BookingResponse>> GetForUserAsync(int userId)
    {
        var bookings = await _bookings.GetForUserAsync(userId);
        return bookings.Select(BookingResponse.From).ToList();
    }

    public async Task<List<BookingResponse>> GetAllAsync()
    {
        var bookings = await _bookings.GetAllAsync();
        return bookings.Select(BookingResponse.From).ToList();
    }

    public async Task<ChangeStatusResult> CancelAsync(int id, int requestingUserId, bool isAdmin)
    {
        var booking = await _bookings.GetByIdAsync(id);
        if (booking is null)
            return ChangeStatusResult.NotFound();
        if (booking.UserId != requestingUserId && !isAdmin)
            return ChangeStatusResult.Forbidden();

        var updated = await _bookings.UpdateStatusAsync(id, BookingStatus.Cancelled);
        return ChangeStatusResult.Updated(BookingResponse.From(updated!));
    }

    public async Task<ChangeStatusResult> ConfirmAsync(int id)
    {
        var updated = await _bookings.UpdateStatusAsync(id, BookingStatus.Confirmed);
        return updated is null
            ? ChangeStatusResult.NotFound()
            : ChangeStatusResult.Updated(BookingResponse.From(updated));
    }
}
