namespace BookingSystem.API.Features.Bookings;

public interface IBookingRepository
{
    Task<BookingWithSlot?> GetByIdAsync(int id);
    Task<List<BookingWithSlot>> GetForUserAsync(int userId);
    Task<List<BookingWithPatient>> GetAllAsync(AdminBookingQuery query);
    Task<BookingStatusCounts> CountByStatusAsync(AdminBookingQuery query);

    // Reads the slot and locks its row for the current transaction (SELECT ... FOR UPDATE),
    // so concurrent bookings for the same slot are serialized during the capacity check.
    Task<SlotInfo?> LockSlotAsync(int slotId);
    Task<int> CountActiveForSlotAsync(int slotId, DateOnly bookingDate);

    Task<BookingWithSlot> CreateAsync(Booking booking);
    Task<BookingWithSlot?> UpdateStatusAsync(int id, BookingStatus status);
}
