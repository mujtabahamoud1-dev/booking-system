namespace BookingSystem.API.Features.Auth;

public enum RefreshFailureReason
{
    TokenNotFound,
    TokenRevoked,
    TokenExpired
}

public class RefreshResult
{
    public bool Succeeded => FailureReason is null;
    public User? User { get; }
    public string? RefreshToken { get; }
    public RefreshFailureReason? FailureReason { get; }

    private RefreshResult(User? user, string? refreshToken, RefreshFailureReason? failureReason)
    {
        User          = user;
        RefreshToken  = refreshToken;
        FailureReason = failureReason;
    }

    public static RefreshResult Success(User user, string refreshToken) => new(user, refreshToken, null);
    public static RefreshResult Failure(RefreshFailureReason reason) => new(null, null, reason);
}
