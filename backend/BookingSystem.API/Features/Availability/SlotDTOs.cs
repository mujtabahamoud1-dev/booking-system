namespace BookingSystem.API.Features.Availability;

public record CreateSlotRequest(int ServiceId, int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime, int MaxBookings);

// ServiceId is intentionally omitted: a slot cannot be moved to a different service.
public record UpdateSlotRequest(int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime, int MaxBookings);

public record SlotResponse(int Id, int ServiceId, int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime, int MaxBookings)
{
    public static SlotResponse From(AvailableSlot s) =>
        new(s.Id, s.ServiceId, s.DayOfWeek, s.StartTime, s.EndTime, s.MaxBookings);
}
