using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var user = await _auth.RegisterAsync(req);
        if (user is null)
            return Conflict(new { message = "Email already in use." });

        var loginResult = await _auth.LoginAsync(new LoginRequest(req.Email, req.Password));
        if (loginResult is null)
            return StatusCode(500);

        var (_, refreshToken) = loginResult.Value;
        SetRefreshTokenCookie(refreshToken);
        return Ok(new AuthResponse(_auth.GenerateAccessToken(user), user.Role, user.Name));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var result = await _auth.LoginAsync(req);
        if (result is null)
            return Unauthorized(new { message = "Invalid email or password." });

        var (user, refreshToken) = result.Value;
        SetRefreshTokenCookie(refreshToken);
        return Ok(new AuthResponse(_auth.GenerateAccessToken(user), user.Role, user.Name));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var token = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new { message = "No refresh token." });

        var result = await _auth.RefreshAsync(token);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid or expired refresh token." });

        SetRefreshTokenCookie(result.RefreshToken!);
        return Ok(new AuthResponse(_auth.GenerateAccessToken(result.User!), result.User!.Role, result.User!.Name));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Cookies["refresh_token"];
        if (!string.IsNullOrEmpty(token))
            await _auth.LogoutAsync(token);

        Response.Cookies.Delete("refresh_token");
        return NoContent();
    }

    private void SetRefreshTokenCookie(string token) =>
        Response.Cookies.Append("refresh_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure   = false,
            SameSite = SameSiteMode.Strict,
            Expires  = DateTimeOffset.UtcNow.AddDays(7)
        });
}
