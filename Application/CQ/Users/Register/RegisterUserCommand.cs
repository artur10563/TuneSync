using Domain.Primitives;
using MediatR;

namespace Application.CQ.Users.Register
{
    public sealed record RegisterUserCommand(string Name, string Email, string Password) : IRequest<Result<Guid>>;
}
