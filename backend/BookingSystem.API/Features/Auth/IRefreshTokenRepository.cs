
namespace BookingSystem.API.Features.Auth;

public interface IRefreshTokenRepository
{
    Task CreateAsync(int userId, string token, DateTime expiresAt);
    Task<(RefreshToken Token, User User)?> GetWithUserAsync(string token);
    Task RevokeByIdAsync(int id);
    Task<bool> RevokeByTokenAsync(string token);
}
