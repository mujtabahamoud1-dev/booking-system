namespace BookingSystem.API.Models;

public class AvailableSlot
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int MaxBookings { get; set; }
}
