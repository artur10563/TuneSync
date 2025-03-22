using Application.DTOs.Auth;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Users.Login
{
    public sealed record LoginUserCommand(string Email, string Password) : IRequest<Result<TokenResponse>>;
}
