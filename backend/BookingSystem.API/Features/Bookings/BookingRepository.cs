using BookingSystem.API.Shared.Database;
using Dapper;
using SqlSearch = BookingSystem.API.Shared.Database.SqlSearch;

namespace BookingSystem.API.Features.Bookings;

public class BookingRepository : IBookingRepository
{
    private const string Columns =
        "id, user_id, service_id, slot_id, booking_date, status, notes, created_at";
    private const string BookingColumns =
        "b.id, b.user_id, b.service_id, b.slot_id, b.booking_date, b.status, b.notes, b.created_at";

    private const string SlotColumns =
        "sl.start_time AS slot_start_time, sl.end_time AS slot_end_time";

    // Inner join: slot_id is a non-null foreign key.
    private const string WithSlot = @"
        FROM bookings b
        JOIN available_slots sl ON sl.id = b.slot_id";

    private readonly DbSession _session;

    public BookingRepository(DbSession session) => _session = session;

    public async Task<BookingWithSlot?> GetByIdAsync(int id)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<BookingWithSlot>(
            $"SELECT {BookingColumns}, {SlotColumns} {WithSlot} WHERE b.id = @id",
            new { id });
    }

    public async Task<List<BookingWithSlot>> GetForUserAsync(int userId)
    {
        var conn = await _session.GetConnectionAsync();
        var bookings = await conn.QueryAsync<BookingWithSlot>($@"
            SELECT {BookingColumns}, {SlotColumns}
            {WithSlot}
            WHERE b.user_id = @userId
            ORDER BY b.booking_date DESC, b.id DESC",
            new { userId });
        return bookings.AsList();
    }

    // Inner joins: a booking whose user or service has been deleted would drop
    // out, but the foreign keys mean such a row cannot exist in the first place.
    // Services is joined for the search, not the projection — an admin looking
    // for "sports massage" is naming the service, not the patient.
    private const string AdminFrom = @"
        FROM bookings b
        JOIN users u             ON u.id  = b.user_id
        JOIN services s          ON s.id  = b.service_id
        JOIN available_slots sl  ON sl.id = b.slot_id";

    // Every parameter is cast explicitly: Postgres cannot infer the type of a
    // parameter that only ever appears next to IS NULL, and these are all
    // optional filters.
    private const string SearchFilter = @"
        (@Search::text IS NULL
         OR u.name    ILIKE @Search::text ESCAPE '\'
         OR u.email   ILIKE @Search::text ESCAPE '\'
         OR u.phone   ILIKE @Search::text ESCAPE '\'
         OR s.name    ILIKE @Search::text ESCAPE '\'
         OR s.name_ar ILIKE @Search::text ESCAPE '\')";

    private const string DateFilter = @"
        (@From::date IS NULL OR b.booking_date >= @From::date)
        AND (@To::date IS NULL OR b.booking_date <= @To::date)";

    private static object Params(AdminBookingQuery q) => new
    {
        Search = SqlSearch.ToPattern(q.Search),
        Status = q.Status?.ToDbValue(),
        q.From,
        q.To
    };

    public async Task<List<BookingWithPatient>> GetAllAsync(AdminBookingQuery query)
    {
        var conn = await _session.GetConnectionAsync();
        var bookings = await conn.QueryAsync<BookingWithPatient>($@"
            SELECT {BookingColumns},
                   {SlotColumns},
                   u.name  AS patient_name,
                   u.email AS patient_email,
                   u.phone AS patient_phone
            {AdminFrom}
            WHERE {SearchFilter}
              AND (@Status::text IS NULL OR b.status = @Status::text)
              AND {DateFilter}
            ORDER BY b.booking_date, b.id",
            Params(query));
        return bookings.AsList();
    }

    // Counted in the database rather than over the returned page, so the numbers
    // stay true no matter how the list is later paged or narrowed.
    public async Task<BookingStatusCounts> CountByStatusAsync(AdminBookingQuery query)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleAsync<BookingStatusCounts>($@"
            SELECT COUNT(*)::int                                          AS total,
                   COUNT(*) FILTER (WHERE b.status = 'pending')::int      AS pending,
                   COUNT(*) FILTER (WHERE b.status = 'confirmed')::int    AS confirmed,
                   COUNT(*) FILTER (WHERE b.status = 'cancelled')::int    AS cancelled
            {AdminFrom}
            WHERE {SearchFilter}
              AND {DateFilter}",
            Params(query));
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

    // CTE because RETURNING cannot join.
    public async Task<BookingWithSlot> CreateAsync(Booking booking)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleAsync<BookingWithSlot>($@"
            WITH b AS (
                INSERT INTO bookings (user_id, service_id, slot_id, booking_date, notes)
                VALUES (@UserId, @ServiceId, @SlotId, @BookingDate, @Notes)
                RETURNING {Columns}
            )
            SELECT {BookingColumns}, {SlotColumns}
            FROM b
            JOIN available_slots sl ON sl.id = b.slot_id",
            booking);
    }

    public async Task<BookingWithSlot?> UpdateStatusAsync(int id, BookingStatus status)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<BookingWithSlot>($@"
            WITH b AS (
                UPDATE bookings SET status = @status WHERE id = @id
                RETURNING {Columns}
            )
            SELECT {BookingColumns}, {SlotColumns}
            FROM b
            JOIN available_slots sl ON sl.id = b.slot_id",
            new { id, status = status.ToDbValue() });
    }
}
