using Application.DTOs.Auth;
using Application.Services.Auth;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Users.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }


    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokens = await _jwtService.RefreshTokenAsync(request.RefreshToken);

        if (string.IsNullOrEmpty(tokens.AccessToken))
            return UserError.InvalidCredentials;
        
        return tokens;
    }
}