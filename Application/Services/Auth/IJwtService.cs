using Application.DTOs.Auth;

namespace Application.Services.Auth
{
    public interface IJwtService
    {
        Task<TokenResponse> GetTokenAsync(string email, string password);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    }
}
