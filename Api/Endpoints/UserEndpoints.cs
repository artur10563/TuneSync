using Application.CQ.Users.Login;
using Application.CQ.Users.Register;
using MediatR;

namespace Api.Endpoints
{
    public static class UserEndpoints
    {
        public static async Task RegisterUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("user/").WithTags("User");

            group.MapPost("register", async (ISender sender, RegisterUserCommand request) =>
            {
                var result = await sender.Send(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Errors);
                }

                return Results.Ok(result.Value);
            }).WithDescription("Register new user");

            group.MapPost("login", async (ISender sender, LoginUserCommand request) =>
            {
                var result = await sender.Send(request);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Errors);
                }
                return Results.Ok(result.Value);
            }).WithDescription("Login");
        }
    }
}
