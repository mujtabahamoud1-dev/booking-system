using Npgsql;

namespace BookingSystem.API.Features.Availability;

public class SlotService : ISlotService
{
    // PostgreSQL SQLSTATE for a foreign_key_violation.
    private const string ForeignKeyViolation = "23503";

    private readonly ISlotRepository _slots;

    public SlotService(ISlotRepository slots) => _slots = slots;

    public async Task<List<SlotResponse>> GetForServiceAsync(int serviceId)
    {
        var slots = await _slots.GetForServiceAsync(serviceId);
        return slots.Select(SlotResponse.From).ToList();
    }

    public async Task<SlotListResponse> SearchAsync(SlotQuery query)
    {
        var slots = await _slots.SearchAsync(query);
        var rows = await _slots.CountByDayAsync(query);

        // Days with no slots do not come back from the GROUP BY, so the array is
        // built from all seven — a zero is exactly the thing an admin is looking
        // for here.
        var byDay = new int[7];
        foreach (var row in rows)
            if (row.DayOfWeek is >= 0 and <= 6) byDay[row.DayOfWeek] = row.Count;

        return new SlotListResponse(
            slots.Select(SlotResponse.From).ToList(),
            new SlotCounts(byDay.Sum(), byDay));
    }

    public async Task<SlotResponse?> GetByIdAsync(int id)
    {
        var slot = await _slots.GetByIdAsync(id);
        return slot is null ? null : SlotResponse.From(slot);
    }

    public async Task<CreateSlotOutcome> CreateAsync(CreateSlotRequest req)
    {
        try
        {
            var slot = await _slots.CreateAsync(new AvailableSlot
            {
                ServiceId   = req.ServiceId,
                DayOfWeek   = req.DayOfWeek,
                StartTime   = req.StartTime,
                EndTime     = req.EndTime,
                MaxBookings = req.MaxBookings
            });
            return new CreateSlotOutcome(CreateSlotStatus.Created, SlotResponse.From(slot));
        }
        catch (PostgresException ex) when (ex.SqlState == ForeignKeyViolation)
        {
            // No service with that id — the service_id FK failed.
            return new CreateSlotOutcome(CreateSlotStatus.ServiceNotFound, null);
        }
    }

    public async Task<SlotResponse?> UpdateAsync(int id, UpdateSlotRequest req)
    {
        var slot = await _slots.UpdateAsync(new AvailableSlot
        {
            Id          = id,
            DayOfWeek   = req.DayOfWeek,
            StartTime   = req.StartTime,
            EndTime     = req.EndTime,
            MaxBookings = req.MaxBookings
        });
        return slot is null ? null : SlotResponse.From(slot);
    }

    public async Task<DeleteSlotResult> DeleteAsync(int id)
    {
        try
        {
            return await _slots.DeleteAsync(id)
                ? DeleteSlotResult.Deleted
                : DeleteSlotResult.NotFound;
        }
        catch (PostgresException ex) when (ex.SqlState == ForeignKeyViolation)
        {
            // The slot is referenced by existing bookings and can't be removed.
            return DeleteSlotResult.InUse;
        }
    }
}
