using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Features.Availability;

[ApiController]
[Route("api/slots")]
public class SlotsController : ControllerBase
{
    private readonly ISlotService _slots;

    public SlotsController(ISlotService slots) => _slots = slots;

    // Public: clients browse the slots offered for a service before booking.
    [HttpGet("service/{serviceId:int}")]
    public async Task<IActionResult> GetForService(int serviceId)
    {
        var slots = await _slots.GetForServiceAsync(serviceId);
        return Ok(slots);
    }

    // The admin week. Omitting serviceId spans every service at once, which is
    // the only way to see that a weekday has no cover from anyone — so this one
    // is admin-only, unlike the per-service route above.
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Search([FromQuery] int? serviceId, [FromQuery] int? dayOfWeek)
    {
        if (dayOfWeek is < 0 or > 6)
            return BadRequest(new { message = "DayOfWeek must be between 0 (Sunday) and 6 (Saturday)." });

        var slots = await _slots.SearchAsync(new SlotQuery(serviceId, dayOfWeek));
        return Ok(slots);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var slot = await _slots.GetByIdAsync(id);
        return slot is null ? NotFound() : Ok(slot);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(CreateSlotRequest req)
    {
        if (!TryValidate(req.DayOfWeek, req.StartTime, req.EndTime, req.MaxBookings, out var error))
            return BadRequest(new { message = error });

        var outcome = await _slots.CreateAsync(req);
        return outcome.Status switch
        {
            CreateSlotStatus.Created         => CreatedAtAction(nameof(GetById), new { id = outcome.Slot!.Id }, outcome.Slot),
            CreateSlotStatus.ServiceNotFound => NotFound(new { message = "Service not found." }),
            _ => StatusCode(500)
        };
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, UpdateSlotRequest req)
    {
        if (!TryValidate(req.DayOfWeek, req.StartTime, req.EndTime, req.MaxBookings, out var error))
            return BadRequest(new { message = error });

        var slot = await _slots.UpdateAsync(id, req);
        return slot is null ? NotFound() : Ok(slot);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _slots.DeleteAsync(id);
        return result switch
        {
            DeleteSlotResult.Deleted  => NoContent(),
            DeleteSlotResult.NotFound => NotFound(),
            DeleteSlotResult.InUse    => Conflict(new
            {
                message = "Slot has existing bookings and cannot be deleted."
            }),
            _ => StatusCode(500)
        };
    }

    private static bool TryValidate(int dayOfWeek, TimeOnly startTime, TimeOnly endTime, int maxBookings, out string error)
    {
        if (dayOfWeek is < 0 or > 6)
        {
            error = "DayOfWeek must be between 0 (Sunday) and 6 (Saturday).";
            return false;
        }
        if (startTime >= endTime)
        {
            error = "StartTime must be earlier than EndTime.";
            return false;
        }
        if (maxBookings < 1)
        {
            error = "MaxBookings must be at least 1.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
