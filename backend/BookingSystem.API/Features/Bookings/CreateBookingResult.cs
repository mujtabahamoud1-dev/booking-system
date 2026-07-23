namespace BookingSystem.API.Features.Bookings;

public enum CreateBookingFailure
{
    DateInPast,
    SlotNotFound,
    ServiceMismatch,
    DayMismatch,
    SlotFull
}

public class CreateBookingResult
{
    public bool Succeeded => Failure is null;
    public BookingResponse? Booking { get; }
    public CreateBookingFailure? Failure { get; }

    private CreateBookingResult(BookingResponse? booking, CreateBookingFailure? failure)
    {
        Booking = booking;
        Failure = failure;
    }

    public static CreateBookingResult Success(BookingResponse booking) => new(booking, null);
    public static CreateBookingResult Fail(CreateBookingFailure failure) => new(null, failure);
}
