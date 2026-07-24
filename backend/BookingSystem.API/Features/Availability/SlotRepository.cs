using BookingSystem.API.Shared.Database;
using Dapper;

namespace BookingSystem.API.Features.Availability;

public class SlotRepository : ISlotRepository
{
    private const string Columns = "id, service_id, day_of_week, start_time, end_time, max_bookings";

    private readonly DbSession _session;

    public SlotRepository(DbSession session) => _session = session;

    public async Task<List<AvailableSlot>> GetForServiceAsync(int serviceId)
    {
        var conn = await _session.GetConnectionAsync();
        var slots = await conn.QueryAsync<AvailableSlot>(
            $"SELECT {Columns} FROM available_slots WHERE service_id = @serviceId ORDER BY day_of_week, start_time",
            new { serviceId });
        return slots.AsList();
    }

    public async Task<AvailableSlot?> GetByIdAsync(int id)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<AvailableSlot>(
            $"SELECT {Columns} FROM available_slots WHERE id = @id",
            new { id });
    }

    public async Task<AvailableSlot> CreateAsync(AvailableSlot slot)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleAsync<AvailableSlot>($@"
            INSERT INTO available_slots (service_id, day_of_week, start_time, end_time, max_bookings)
            VALUES (@ServiceId, @DayOfWeek, @StartTime, @EndTime, @MaxBookings)
            RETURNING {Columns}",
            slot);
    }

    public async Task<AvailableSlot?> UpdateAsync(AvailableSlot slot)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<AvailableSlot>($@"
            UPDATE available_slots
            SET day_of_week = @DayOfWeek, start_time = @StartTime, end_time = @EndTime, max_bookings = @MaxBookings
            WHERE id = @Id
            RETURNING {Columns}",
            slot);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.ExecuteAsync("DELETE FROM available_slots WHERE id = @id", new { id }) > 0;
    }
}
