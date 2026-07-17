
namespace BookingSystem.API.Features.Auth;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterRequest req);
    Task<(User User, string RefreshToken)?> LoginAsync(LoginRequest req);
    Task<RefreshResult> RefreshAsync(string token);
    Task<bool> LogoutAsync(string token);
    string GenerateAccessToken(User user);
}
