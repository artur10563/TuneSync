namespace Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static async Task RegisterAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var auth = app.MapGroup("api/auth");

            auth.MapPost("/register", () => { });
            auth.MapGet("/login", () => { });
        }
    }
}
