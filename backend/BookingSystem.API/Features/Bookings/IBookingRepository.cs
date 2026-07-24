namespace BookingSystem.API.Features.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    Task<List<Booking>> GetForUserAsync(int userId);
    Task<List<Booking>> GetAllAsync();

    // Reads the slot and locks its row for the current transaction (SELECT ... FOR UPDATE),
    // so concurrent bookings for the same slot are serialized during the capacity check.
    Task<SlotInfo?> LockSlotAsync(int slotId);
    Task<int> CountActiveForSlotAsync(int slotId, DateOnly bookingDate);

    Task<Booking> CreateAsync(Booking booking);
    Task<Booking?> UpdateStatusAsync(int id, BookingStatus status);
}
