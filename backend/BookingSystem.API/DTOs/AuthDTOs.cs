namespace BookingSystem.API.DTOs;

public record RegisterRequest(string Name, string Email, string Password, string? Phone);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string AccessToken, string Role, string Name);
