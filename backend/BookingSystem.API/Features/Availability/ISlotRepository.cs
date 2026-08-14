namespace BookingSystem.API.Features.Availability;

public interface ISlotRepository
{
    Task<List<AvailableSlot>> GetForServiceAsync(int serviceId);
    Task<List<AvailableSlot>> SearchAsync(SlotQuery query);
    Task<List<DayCount>> CountByDayAsync(SlotQuery query);
    Task<AvailableSlot?> GetByIdAsync(int id);
    Task<AvailableSlot> CreateAsync(AvailableSlot slot);
    Task<AvailableSlot?> UpdateAsync(AvailableSlot slot);
    Task<bool> DeleteAsync(int id);
}
