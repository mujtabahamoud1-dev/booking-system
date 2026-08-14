namespace BookingSystem.API.Features.Availability;

public record CreateSlotRequest(int ServiceId, int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime, int MaxBookings);

// ServiceId is intentionally omitted: a slot cannot be moved to a different service.
public record UpdateSlotRequest(int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime, int MaxBookings);

public record SlotResponse(int Id, int ServiceId, int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime, int MaxBookings)
{
    public static SlotResponse From(AvailableSlot s) =>
        new(s.Id, s.ServiceId, s.DayOfWeek, s.StartTime, s.EndTime, s.MaxBookings);
}

// The admin week's filters. A null ServiceId means every service at once, which
// is the only way to see that a weekday has no cover from anyone.
public record SlotQuery(int? ServiceId = null, int? DayOfWeek = null);

// Seven entries, Sunday first, so a day with no cover reports a real zero rather
// than going missing from the response.
public record SlotCounts(int Total, int[] ByDay);

public record SlotListResponse(List<SlotResponse> Items, SlotCounts Counts);

// One row of the group-by behind SlotCounts.
public record DayCount(int DayOfWeek, int Count);
