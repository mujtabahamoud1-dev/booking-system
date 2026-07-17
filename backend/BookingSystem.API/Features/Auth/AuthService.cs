using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BookingSystem.API.Shared.Database;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookingSystem.API.Features.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _tokens;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly JwtSettings _jwt;

    public AuthService(
        IUserRepository users,
        IRefreshTokenRepository tokens,
        IUnitOfWorkFactory unitOfWorkFactory,
        IOptions<JwtSettings> jwt)
    {
        _users             = users;
        _tokens            = tokens;
        _unitOfWorkFactory = unitOfWorkFactory;
        _jwt               = jwt.Value;
    }

    public async Task<User?> RegisterAsync(RegisterRequest req)
    {
        if (await _users.GetByEmailAsync(req.Email) is not null)
            return null;

        var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);
        return await _users.CreateAsync(req.Name, req.Email, hash, req.Phone);
    }

    public async Task<(User User, string RefreshToken)?> LoginAsync(LoginRequest req)
    {
        var user = await _users.GetByEmailAsync(req.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return null;

        var refreshToken = GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);
        await _tokens.CreateAsync(user.Id, refreshToken, expiresAt);

        return (user, refreshToken);
    }

    public async Task<RefreshResult> RefreshAsync(string token)
    {
        var result = await _tokens.GetWithUserAsync(token);
        if (result is null)
            return RefreshResult.Failure(RefreshFailureReason.TokenNotFound);

        var (refreshToken, user) = result.Value;
        if (refreshToken.IsRevoked)
            return RefreshResult.Failure(RefreshFailureReason.TokenRevoked);
        if (refreshToken.ExpiresAt < DateTime.UtcNow)
            return RefreshResult.Failure(RefreshFailureReason.TokenExpired);

        var newToken = await _unitOfWorkFactory.ExecuteAsync(async uow =>
        {
            await _tokens.RevokeByIdAsync(refreshToken.Id, uow);
            var generated = GenerateRefreshToken();
            await _tokens.CreateAsync(user.Id, generated, DateTime.UtcNow.AddDays(RefreshTokenExpiryDays), uow);
            return generated;
        });

        return RefreshResult.Success(user, newToken);
    }

    public async Task<bool> LogoutAsync(string token) =>
        await _tokens.RevokeByTokenAsync(token);

    public string GenerateAccessToken(User user)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             _jwt.Issuer,
            audience:           _jwt.Audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int RefreshTokenExpiryDays => _jwt.RefreshTokenExpiryDays;

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
