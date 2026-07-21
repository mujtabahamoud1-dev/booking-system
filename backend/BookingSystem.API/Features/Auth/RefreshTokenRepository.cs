using BookingSystem.API.Shared.Database;
using Dapper;

namespace BookingSystem.API.Features.Auth;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly DatabaseConnection _db;

    public RefreshTokenRepository(DatabaseConnection db) => _db = db;

    public async Task CreateAsync(int userId, string token, DateTime expiresAt, IUnitOfWork? uow = null)
    {
        await using var owned = uow is null ? await _db.OpenAsync() : null;
        var conn = uow?.Connection ?? owned!;

        await conn.ExecuteAsync(@"
            INSERT INTO refresh_tokens (user_id, token, expires_at)
            VALUES (@userId, @token, @expiresAt)",
            new { userId, token, expiresAt },
            uow?.Transaction);
    }

    public async Task<(RefreshToken Token, User User)?> GetWithUserAsync(string token)
    {
        await using var conn = await _db.OpenAsync();
        const string sql = @"
            SELECT rt.id, rt.user_id, rt.token, rt.expires_at, rt.is_revoked, rt.created_at,
                   u.id, u.name, u.email, u.password_hash, u.phone, u.role, u.created_at
            FROM refresh_tokens rt
            JOIN users u ON u.id = rt.user_id
            WHERE rt.token = @token";

        // Multi-map: each row is split at the second "id" column into a RefreshToken and its User.
        var rows = await conn.QueryAsync<RefreshToken, User, (RefreshToken Token, User User)>(
            sql,
            (rt, u) => (rt, u),
            new { token },
            splitOn: "id");

        var list = rows.ToList();
        return list.Count == 0 ? null : list[0];
    }

    public async Task RevokeByIdAsync(int id, IUnitOfWork? uow = null)
    {
        await using var owned = uow is null ? await _db.OpenAsync() : null;
        var conn = uow?.Connection ?? owned!;

        await conn.ExecuteAsync(
            "UPDATE refresh_tokens SET is_revoked = TRUE WHERE id = @id",
            new { id },
            uow?.Transaction);
    }

    public async Task<bool> RevokeByTokenAsync(string token)
    {
        await using var conn = await _db.OpenAsync();
        var affected = await conn.ExecuteAsync(
            "UPDATE refresh_tokens SET is_revoked = TRUE WHERE token = @token",
            new { token });
        return affected > 0;
    }
}
