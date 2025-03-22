using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services.Auth;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;
using MediatR;


namespace Application.CQ.Users.Register
{
    internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IAuthService _auth;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<RegisterUserCommand> _validator;

        public RegisterUserCommandHandler(IAuthService auth, IUnitOfWork uow, IValidator<RegisterUserCommand> validator)
        {
            _auth = auth;
            _uow = uow;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.AsErrors();


            User user = null!;
            try
            {
                //add to firebase
                var identityId = await _auth.RegisterAsync(request.Email, request.Password);

                try
                {
                    //store to database
                    user = new User(request.Name, request.Email, identityId);
                    _uow.UserRepository.Insert(user);
                    await _uow.SaveChangesAsync();
                }
                catch (Exception)
                {
                    //Database failure. Delete newly created firebase user
                    await _auth.DeleteAsync(identityId);
                    return new Error("Failed to create user");
                }
            }
            catch (Exception)
            {
                //Firebase failure
                return new Error("Failed to create user");
            }

            return Result.Success(user.Guid);
        }
    }
}
