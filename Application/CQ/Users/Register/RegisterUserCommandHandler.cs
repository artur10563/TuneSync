using Application.Repositories.Shared;
using Application.Services.Auth;
using Domain.Entities;
using Domain.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQ.Users.Register
{
    internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IAuthService _auth;
        private readonly IUnitOfWork _uow;

        public RegisterUserCommandHandler(IAuthService auth, IUnitOfWork uow)
        {
            _auth = auth;
            _uow = uow;
        }

        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            //TODO: VALIDATIOn

            //add to firebase
            var identityId = await _auth.RegisterAsync(request.Email, request.Password);

            //store to database
            var user = new User(request.Name, request.Email, identityId);

            _uow.UserRepository.Insert(user);
            await _uow.SaveChangesAsync();

            return Result.Success(user.Guid);
        }
    }
}
