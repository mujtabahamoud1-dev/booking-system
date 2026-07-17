using BookingSystem.API.Shared.Database;

namespace BookingSystem.API.Features.Auth;

public interface IRefreshTokenRepository
{
    Task CreateAsync(int userId, string token, DateTime expiresAt, IUnitOfWork? uow = null);
    Task<(RefreshToken Token, User User)?> GetWithUserAsync(string token);
    Task RevokeByIdAsync(int id, IUnitOfWork? uow = null);
    Task<bool> RevokeByTokenAsync(string token);
}
