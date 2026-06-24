
namespace BookingSystem.API.Features.Auth;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> CreateAsync(string name, string email, string passwordHash, string? phone);
}
