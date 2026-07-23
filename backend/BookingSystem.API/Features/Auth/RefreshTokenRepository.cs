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
            new { userId, token, expiresAt }, uow?.Transaction);
    }

    public async Task<(RefreshToken Token, User User)?> GetWithUserAsync(string token)
    {
        await using var conn = await _db.OpenAsync();
        var rows = await conn.QueryAsync<RefreshToken, User, (RefreshToken Token, User User)>(@"
            SELECT rt.id, rt.expires_at, rt.is_revoked,
                   u.id, u.name, u.email, u.password_hash, u.phone, u.role, u.created_at
            FROM refresh_tokens rt
            JOIN users u ON u.id = rt.user_id
            WHERE rt.token = @token",
            (rt, u) => (rt, u),
            new { token },
            splitOn: "id");

        var row = rows.AsList();
        return row.Count == 0 ? null : row[0];
    }

    public async Task RevokeByIdAsync(int id, IUnitOfWork? uow = null)
    {
        await using var owned = uow is null ? await _db.OpenAsync() : null;
        var conn = uow?.Connection ?? owned!;

        await conn.ExecuteAsync(
            "UPDATE refresh_tokens SET is_revoked = TRUE WHERE id = @id",
            new { id }, uow?.Transaction);
    }

    public async Task<bool> RevokeByTokenAsync(string token)
    {
        await using var conn = await _db.OpenAsync();
        return await conn.ExecuteAsync(
            "UPDATE refresh_tokens SET is_revoked = TRUE WHERE token = @token",
            new { token }) > 0;
    }
}
