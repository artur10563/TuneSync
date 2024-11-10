namespace Application.Services.Auth
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(string email, string password);
    }
}
