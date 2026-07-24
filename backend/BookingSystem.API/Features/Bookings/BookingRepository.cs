using BookingSystem.API.Shared.Database;
using Dapper;

namespace BookingSystem.API.Features.Bookings;

public class BookingRepository : IBookingRepository
{
    private const string Columns =
        "id, user_id, service_id, slot_id, booking_date, status, notes, created_at";

    private readonly DbSession _session;

    public BookingRepository(DbSession session) => _session = session;

    public async Task<Booking?> GetByIdAsync(int id)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<Booking>(
            $"SELECT {Columns} FROM bookings WHERE id = @id",
            new { id });
    }

    public async Task<List<Booking>> GetForUserAsync(int userId)
    {
        var conn = await _session.GetConnectionAsync();
        var bookings = await conn.QueryAsync<Booking>(
            $"SELECT {Columns} FROM bookings WHERE user_id = @userId ORDER BY booking_date DESC, id DESC",
            new { userId });
        return bookings.AsList();
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        var conn = await _session.GetConnectionAsync();
        var bookings = await conn.QueryAsync<Booking>(
            $"SELECT {Columns} FROM bookings ORDER BY booking_date DESC, id DESC");
        return bookings.AsList();
    }

    public async Task<SlotInfo?> LockSlotAsync(int slotId)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<SlotInfo>(
            "SELECT service_id, day_of_week, max_bookings FROM available_slots WHERE id = @slotId FOR UPDATE",
            new { slotId });
    }

    public async Task<int> CountActiveForSlotAsync(int slotId, DateOnly bookingDate)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.ExecuteScalarAsync<int>(@"
            SELECT COUNT(*) FROM bookings
            WHERE slot_id = @slotId AND booking_date = @bookingDate AND status <> @cancelled",
            new { slotId, bookingDate, cancelled = BookingStatus.Cancelled.ToDbValue() });
    }

    public async Task<Booking> CreateAsync(int userId, int serviceId, int slotId, DateOnly bookingDate, string? notes)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleAsync<Booking>($@"
            INSERT INTO bookings (user_id, service_id, slot_id, booking_date, notes)
            VALUES (@userId, @serviceId, @slotId, @bookingDate, @notes)
            RETURNING {Columns}",
            new { userId, serviceId, slotId, bookingDate, notes });
    }

    public async Task<Booking?> UpdateStatusAsync(int id, BookingStatus status)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<Booking>($@"
            UPDATE bookings SET status = @status WHERE id = @id
            RETURNING {Columns}",
            new { id, status = status.ToDbValue() });
    }
}
