using Application.Services.Auth;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Infrastructure.Services.Auth
{
    public sealed class JwtService : IJwtService
    {
        private readonly HttpClient _httpClient;

        public JwtService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<string> GetTokenAsync(string email, string password)
        {
            var request = new
            {
                email,
                password,
                returnSecureToken = true
            };
            var response = await _httpClient.PostAsJsonAsync("", request);
            var token = await response.Content.ReadFromJsonAsync<AuthToken>();

            return token.IdToken;
        }


        public class AuthToken
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
