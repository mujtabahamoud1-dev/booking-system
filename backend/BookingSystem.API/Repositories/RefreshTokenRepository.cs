using BookingSystem.API.Database;
using BookingSystem.API.Models;
using Npgsql;

namespace BookingSystem.API.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly DatabaseConnection _db;

    public RefreshTokenRepository(DatabaseConnection db) => _db = db;

    public async Task CreateAsync(int userId, string token, DateTime expiresAt)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = new NpgsqlCommand(@"
            INSERT INTO refresh_tokens (user_id, token, expires_at)
            VALUES (@userId, @token, @expiresAt)", conn);

        cmd.Parameters.AddWithValue("userId", userId);
        cmd.Parameters.AddWithValue("token", token);
        cmd.Parameters.AddWithValue("expiresAt", expiresAt);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<(RefreshToken Token, User User)?> GetWithUserAsync(string token)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = new NpgsqlCommand(@"
            SELECT rt.id, rt.expires_at, rt.is_revoked,
                   u.id, u.name, u.email, u.password_hash, u.phone, u.role, u.created_at
            FROM refresh_tokens rt
            JOIN users u ON u.id = rt.user_id
            WHERE rt.token = @token", conn);
        cmd.Parameters.AddWithValue("token", token);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        var refreshToken = new RefreshToken
        {
            Id        = reader.GetInt32(0),
            ExpiresAt = reader.GetDateTime(1),
            IsRevoked = reader.GetBoolean(2)
        };
        var user = new User
        {
            Id           = reader.GetInt32(3),
            Name         = reader.GetString(4),
            Email        = reader.GetString(5),
            PasswordHash = reader.GetString(6),
            Phone        = reader.IsDBNull(7) ? null : reader.GetString(7),
            Role         = reader.GetString(8),
            CreatedAt    = reader.GetDateTime(9)
        };

        return (refreshToken, user);
    }

    public async Task RevokeByIdAsync(int id)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "UPDATE refresh_tokens SET is_revoked = TRUE WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<bool> RevokeByTokenAsync(string token)
    {
        await using var conn = await _db.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "UPDATE refresh_tokens SET is_revoked = TRUE WHERE token = @token", conn);
        cmd.Parameters.AddWithValue("token", token);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}
