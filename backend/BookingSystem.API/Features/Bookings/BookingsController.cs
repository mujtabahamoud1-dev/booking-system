using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Features.Bookings;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookings;

    public BookingsController(IBookingService bookings) => _bookings = bookings;

    // A client books a slot for themselves; the user id comes from the token, not the request.
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest req)
    {
        var result = await _bookings.CreateAsync(CurrentUserId, req);
        if (result.Succeeded)
            return CreatedAtAction(nameof(GetMine), null, result.Booking);

        return result.Failure switch
        {
            CreateBookingFailure.DateInPast      => BadRequest(new { message = "Booking date is in the past." }),
            CreateBookingFailure.SlotNotFound    => NotFound(new { message = "Slot not found." }),
            CreateBookingFailure.ServiceMismatch => BadRequest(new { message = "Slot does not belong to that service." }),
            CreateBookingFailure.DayMismatch     => BadRequest(new { message = "Booking date does not fall on the slot's day of week." }),
            CreateBookingFailure.SlotFull        => Conflict(new { message = "Slot is fully booked for that date." }),
            _ => StatusCode(500)
        };
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var bookings = await _bookings.GetForUserAsync(CurrentUserId);
        return Ok(bookings);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _bookings.GetAllAsync();
        return Ok(bookings);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _bookings.CancelAsync(id, CurrentUserId, User.IsInRole("admin"));
        return result.Status switch
        {
            ChangeStatus.Updated   => Ok(result.Booking),
            ChangeStatus.NotFound  => NotFound(),
            ChangeStatus.Forbidden => Forbid(),
            _ => StatusCode(500)
        };
    }

    [HttpPost("{id:int}/confirm")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Confirm(int id)
    {
        var result = await _bookings.ConfirmAsync(id);
        return result.Status == ChangeStatus.Updated ? Ok(result.Booking) : NotFound();
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
