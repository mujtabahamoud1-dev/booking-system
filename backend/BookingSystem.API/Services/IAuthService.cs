using BookingSystem.API.DTOs;
using BookingSystem.API.Models;

namespace BookingSystem.API.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterRequest req);
    Task<(User User, string RefreshToken)?> LoginAsync(LoginRequest req);
    Task<(User User, string RefreshToken)?> RefreshAsync(string token);
    Task<bool> LogoutAsync(string token);
    string GenerateAccessToken(User user);
}
