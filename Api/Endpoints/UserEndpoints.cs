﻿using Application.CQ.Users.Login;
using Application.CQ.Users.RefreshToken;
using Application.CQ.Users.Register;
using Application.DTOs.Auth;
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
            
            return app;
        }
    }
}