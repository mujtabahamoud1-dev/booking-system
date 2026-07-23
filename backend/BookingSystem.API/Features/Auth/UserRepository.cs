using BookingSystem.API.Shared.Database;
using Dapper;

namespace BookingSystem.API.Features.Auth;

public class UserRepository : IUserRepository
{
    private readonly DbSession _session;

    public UserRepository(DbSession session) => _session = session;

    public async Task<User?> GetByEmailAsync(string email)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<User>(
            "SELECT id, name, email, password_hash, phone, role, created_at FROM users WHERE email = @email",
            new { email });
    }

    public async Task<User?> CreateAsync(string name, string email, string passwordHash, string? phone)
    {
        var conn = await _session.GetConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<User>(@"
            INSERT INTO users (name, email, password_hash, phone, role)
            VALUES (@name, @email, @passwordHash, @phone, 'client')
            RETURNING id, name, email, password_hash, phone, role, created_at",
            new { name, email, passwordHash, phone });
    }
}
