using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Features.Services;

[ApiController]
[Route("api/services")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _services;

    public ServicesController(IServiceService services) => _services = services;

    // Public: clients only see active services. Admins can pass ?includeInactive=true
    // to also list deactivated ones.
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var canSeeInactive = includeInactive && User.IsInRole("admin");
        var services = await _services.GetAllAsync(canSeeInactive);
        return Ok(services);
    }

    // The admin list, filtered in the database. Kept apart from the public
    // endpoint above so that one can stay a plain array for patients while this
    // one carries the counts the admin filter chips need.
    [HttpGet("admin")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] string? status)
    {
        if (!TryParseActive(status, out var isActive))
            return BadRequest(new { message = "Unknown service status." });

        var services = await _services.SearchAsync(new ServiceQuery(search, isActive));
        return Ok(services);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var service = await _services.GetByIdAsync(id);
        return service is null ? NotFound() : Ok(service);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(CreateServiceRequest req)
    {
        if (!TryValidate(req.Name, req.Duration, req.Price, out var error))
            return BadRequest(new { message = error });

        var service = await _services.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, UpdateServiceRequest req)
    {
        if (!TryValidate(req.Name, req.Duration, req.Price, out var error))
            return BadRequest(new { message = error });

        var service = await _services.UpdateAsync(id, req);
        return service is null ? NotFound() : Ok(service);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _services.DeleteAsync(id);
        return result switch
        {
            DeleteServiceResult.Deleted  => NoContent(),
            DeleteServiceResult.NotFound => NotFound(),
            DeleteServiceResult.InUse    => Conflict(new
            {
                message = "Service has existing bookings and cannot be deleted. Deactivate it instead."
            }),
            _ => StatusCode(500)
        };
    }

    // An unrecognised status is rejected rather than ignored: silently returning
    // everything would read as "nothing has been deactivated".
    private static bool TryParseActive(string? value, out bool? isActive)
    {
        isActive = value switch
        {
            "active"   => true,
            "inactive" => false,
            _ => null
        };
        return isActive is not null || string.IsNullOrWhiteSpace(value) || value == "all";
    }

    private static bool TryValidate(string name, int duration, decimal price, out string error)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Name is required.";
            return false;
        }
        if (duration <= 0)
        {
            error = "Duration must be greater than zero.";
            return false;
        }
        if (price < 0)
        {
            error = "Price cannot be negative.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
