using Application.DTOs.Auth;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Users.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokenResponse>>;