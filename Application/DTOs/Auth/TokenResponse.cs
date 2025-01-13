namespace Application.DTOs.Auth;

public sealed record TokenResponse(string AccessToken, string RefreshToken, long ExpiresAt)
{
    public static TokenResponse CreateUnixTime(string accessToken, string refreshToken, long expiresAt)
    {
        return new TokenResponse(accessToken, refreshToken, DateTimeOffset.UtcNow.ToUnixTimeSeconds() + expiresAt);
    }
}