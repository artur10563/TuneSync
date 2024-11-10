namespace Application.Services.Auth
{
    public interface IJwtService
    {
        Task<string> GetTokenAsync(string email, string password);
    }
}
