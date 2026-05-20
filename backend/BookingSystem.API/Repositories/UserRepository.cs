using BookingSystem.API.Database;
using BookingSystem.API.Models;
using Npgsql;

namespace BookingSystem.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseConnection _db;

    public UserRepository(DatabaseConnection db) => _db = db;

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT id, name, email, password_hash, phone, role, created_at FROM users WHERE email = @email", conn);
        cmd.Parameters.AddWithValue("email", email);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUser(reader) : null;
    }

    public async Task<User?> CreateAsync(string name, string email, string passwordHash, string? phone)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = new NpgsqlCommand(@"
            INSERT INTO users (name, email, password_hash, phone, role)
            VALUES (@name, @email, @hash, @phone, 'client')
            RETURNING id, name, email, password_hash, phone, role, created_at", conn);

        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("hash", passwordHash);
        cmd.Parameters.AddWithValue("phone", phone ?? (object)DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapUser(reader) : null;
    }

    private static User MapUser(NpgsqlDataReader r) => new()
    {
        Id           = r.GetInt32(0),
        Name         = r.GetString(1),
        Email        = r.GetString(2),
        PasswordHash = r.GetString(3),
        Phone        = r.IsDBNull(4) ? null : r.GetString(4),
        Role         = r.GetString(5),
        CreatedAt    = r.GetDateTime(6)
    };
}
