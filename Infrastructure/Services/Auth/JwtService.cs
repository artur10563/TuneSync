using Application.Services.Auth;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Application.DTOs.Auth;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Auth
{
    public sealed class JwtService : IJwtService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public JwtService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }


        public async Task<TokenResponse> GetTokenAsync(string email, string password)
        {
            var request = new
            {
                email,
                password,
                returnSecureToken = true
            };
            var url = _configuration["Auth:TokenUrl"];
            var response = await _httpClient.PostAsJsonAsync(url, request);
            var token = await response.Content.ReadFromJsonAsync<AuthTokenResponse>();

            return TokenResponse.CreateUnixTime(token.IdToken, token.RefreshToken, token.ExpiresIn);
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var request = new
            {
                grant_type = "refresh_token",
                refresh_token = refreshToken
            };
    
            var url = _configuration["Auth:RefreshTokenUrl"];
            var response = await _httpClient.PostAsJsonAsync(url, request);
            var token = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();

            return TokenResponse.CreateUnixTime(token.IdToken, token.RefreshToken, token.ExpiresIn);
        }

        public class RefreshTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
            
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
            
            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }
            
            [JsonPropertyName("id_token")]
            public string IdToken { get; set; }
            
            [JsonPropertyName("user_id")]
            public string UserId { get; set; }
            
            [JsonPropertyName("project_id")]
            public string ProjectId { get; set; }
        }
        public class AuthTokenResponse
        {
            [JsonPropertyName("kind")]
            public string Kind { get; set; }

            [JsonPropertyName("localId")]
            public string LocalId { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("displayName")]
            public string DisplayName { get; set; }

            [JsonPropertyName("idToken")]
            public string IdToken { get; set; }

            [JsonPropertyName("registered")]
            public bool Registered { get; set; }

            [JsonPropertyName("refreshToken")]
            public string RefreshToken { get; set; }

            [JsonPropertyName("expiresIn")]
            public long ExpiresIn { get; set; }
        }
    }
}
