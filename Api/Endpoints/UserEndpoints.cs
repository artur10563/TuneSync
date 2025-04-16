using Api.Extensions;
using Application.CQ.Users.Login;
using Application.CQ.Users.RefreshToken;
using Application.CQ.Users.Register;
using Application.DTOs.Auth;
using Application.Repositories.Shared;
using Domain.Primitives;
using MediatR;

namespace Api.Endpoints
{
    public static class UserEndpoints
    {
        public static IEndpointRouteBuilder RegisterUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/user/").WithTags("User");

            group.MapPost("register", async (ISender sender, RegisterUserCommand request) =>
            {
                var result = await sender.Send(request);
                return result.IsFailure
                    ? Results.BadRequest(result.Errors)
                    : Results.Ok(result.Value);
            }).WithDescription("Register new user").Produces<Guid>();

            group.MapPost("login", async (ISender sender, LoginUserCommand request) =>
            {
                var result = await sender.Send(request);
                return result.IsFailure
                    ? Results.BadRequest(result.Errors)
                    : Results.Ok(result.Value);
            }).WithDescription("Login").Produces<TokenResponse>();

            group.MapPost("refresh", async (ISender sender, RefreshTokenCommand request) =>
            {
                var result = await sender.Send(request);
                return result.IsFailure
                    ? Results.BadRequest(result.Errors)
                    : Results.Ok(result.Value);
            }).WithDescription("Refresh the token").Produces<TokenResponse>();;
            
            group.MapGet("roles", async (IUnitOfWork _uow, HttpContext httpContext) =>
            {
                var user = await httpContext.GetCurrentUserAsync();
                return Results.Ok(new[] { user?.Role ?? GlobalVariables.UserConstants.Roles.User });
            }).RequireAuthorization().WithDescription("Returns user roles").Produces<List<string>>();
            
            return app;
        }
    }
}