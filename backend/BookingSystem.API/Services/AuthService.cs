using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BookingSystem.API.Database;
using BookingSystem.API.DTOs;
using BookingSystem.API.Models;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace BookingSystem.API.Services;

public class AuthService
{
    private readonly DatabaseConnection _db;
    private readonly IConfiguration _config;

    public AuthService(DatabaseConnection db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<User?> RegisterAsync(RegisterRequest req)
    {
        await using var conn = await _db.OpenAsync();

        await using var check = new NpgsqlCommand(
            "SELECT id FROM users WHERE email = @email", conn);
        check.Parameters.AddWithValue("email", req.Email);
        if (await check.ExecuteScalarAsync() is not null)
            return null;

        var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        await using var cmd = new NpgsqlCommand(@"
            INSERT INTO users (name, email, password_hash, phone, role)
            VALUES (@name, @email, @hash, @phone, 'client')
            RETURNING id, name, email, password_hash, phone, role, created_at", conn);

        cmd.Parameters.AddWithValue("name", req.Name);
        cmd.Parameters.AddWithValue("email", req.Email);
        cmd.Parameters.AddWithValue("hash", hash);
        cmd.Parameters.AddWithValue("phone", req.Phone ?? (object)DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;
        return MapUser(reader);
    }

    public async Task<(User user, string refreshToken)?> LoginAsync(LoginRequest req)
    {
        await using var conn = await _db.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, name, email, password_hash, phone, role, created_at FROM users WHERE email = @email", conn);
        cmd.Parameters.AddWithValue("email", req.Email);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        var user = MapUser(reader);
        await reader.CloseAsync();

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return null;

        var refreshToken = GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpiryDays"]!));

        await using var insert = new NpgsqlCommand(@"
            INSERT INTO refresh_tokens (user_id, token, expires_at)
            VALUES (@userId, @token, @expiresAt)", conn);
        insert.Parameters.AddWithValue("userId", user.Id);
        insert.Parameters.AddWithValue("token", refreshToken);
        insert.Parameters.AddWithValue("expiresAt", expiresAt);
        await insert.ExecuteNonQueryAsync();

        return (user, refreshToken);
    }

    public async Task<(User user, string refreshToken)?> RefreshAsync(string token)
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

        var tokenId = reader.GetInt32(0);
        var expiresAt = reader.GetDateTime(1);
        var isRevoked = reader.GetBoolean(2);
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
        await reader.CloseAsync();

        if (isRevoked || expiresAt < DateTime.UtcNow) return null;

        await using var revoke = new NpgsqlCommand(
            "UPDATE refresh_tokens SET is_revoked = TRUE WHERE id = @id", conn);
        revoke.Parameters.AddWithValue("id", tokenId);
        await revoke.ExecuteNonQueryAsync();

        var newToken = GenerateRefreshToken();
        var newExpiry = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpiryDays"]!));

        await using var insert = new NpgsqlCommand(@"
            INSERT INTO refresh_tokens (user_id, token, expires_at)
            VALUES (@userId, @token, @expiresAt)", conn);
        insert.Parameters.AddWithValue("userId", user.Id);
        insert.Parameters.AddWithValue("token", newToken);
        insert.Parameters.AddWithValue("expiresAt", newExpiry);
        await insert.ExecuteNonQueryAsync();

        return (user, newToken);
    }

    public async Task<bool> LogoutAsync(string token)
    {
        await using var conn = await _db.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "UPDATE refresh_tokens SET is_revoked = TRUE WHERE token = @token", conn);
        cmd.Parameters.AddWithValue("token", token);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenExpiryMinutes"]!)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

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
