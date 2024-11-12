using Application.Services.Auth;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Users.Login
{
    internal sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<string>>
    {
        private readonly IJwtService _jwtService;
        public LoginUserCommandHandler(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var token = await _jwtService.GetTokenAsync(request.Email, request.Password);
            if (string.IsNullOrEmpty(token))
                return UserError.InvalidCredentials;

            return token;
        }
    }
}
