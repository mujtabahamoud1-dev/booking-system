using BookingSystem.API.Shared.Database;
using Dapper;

namespace BookingSystem.API.Features.Auth;

public class UserRepository : IUserRepository
{
    private readonly DatabaseConnection _db;

    public UserRepository(DatabaseConnection db) => _db = db;

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var conn = await _db.OpenAsync();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT id, name, email, password_hash, phone, role, created_at FROM users WHERE email = @email",
            new { email });
    }

    public async Task<User?> CreateAsync(string name, string email, string passwordHash, string? phone)
    {
        await using var conn = await _db.OpenAsync();
        return await conn.QuerySingleAsync<User>(@"
            INSERT INTO users (name, email, password_hash, phone, role)
            VALUES (@name, @email, @passwordHash, @phone, 'client')
            RETURNING id, name, email, password_hash, phone, role, created_at",
            new { name, email, passwordHash, phone });
    }
}
