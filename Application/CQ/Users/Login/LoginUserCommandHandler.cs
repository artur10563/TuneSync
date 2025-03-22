using Application.DTOs.Auth;
using Application.Services.Auth;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Users.Login
{
    internal sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<TokenResponse>>
    {
        private readonly IJwtService _jwtService;
        public LoginUserCommandHandler(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task<Result<TokenResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var tokens = await _jwtService.GetTokenAsync(request.Email, request.Password);
            if (string.IsNullOrEmpty(tokens.AccessToken))
                return UserError.InvalidCredentials;

            return tokens;
        }
    }
}
