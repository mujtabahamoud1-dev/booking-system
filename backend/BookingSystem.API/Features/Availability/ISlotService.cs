namespace BookingSystem.API.Features.Availability;

public enum CreateSlotStatus
{
    Created,
    ServiceNotFound
}

public record CreateSlotOutcome(CreateSlotStatus Status, SlotResponse? Slot);

public enum DeleteSlotResult
{
    Deleted,
    NotFound,
    InUse
}

public interface ISlotService
{
    Task<List<SlotResponse>> GetForServiceAsync(int serviceId);
    Task<SlotResponse?> GetByIdAsync(int id);
    Task<CreateSlotOutcome> CreateAsync(CreateSlotRequest req);
    Task<SlotResponse?> UpdateAsync(int id, UpdateSlotRequest req);
    Task<DeleteSlotResult> DeleteAsync(int id);
}
