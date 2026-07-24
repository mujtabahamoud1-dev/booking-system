
namespace BookingSystem.API.Features.Auth;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> CreateAsync(User user);
}
