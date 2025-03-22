using Application.Repositories.Shared;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;

namespace Application.CQ.Users.Register
{
    public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator(IUnitOfWork _uow)
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage(UserError.InvalidEmail.Description);

            RuleFor(x => x.Email).MustAsync(async (email, cancelationToken) =>
            {
                return await _uow.UserRepository.IsEmailUniqueAsync(email);
            }).WithMessage(UserError.UserExists.Description);

            RuleFor(x => x.Name).Length(
                min: GlobalVariables.UserConstants.NameMinLength,
                max: GlobalVariables.UserConstants.NameMaxLength)
                .WithMessage(UserError.NameRequired.Description);

            RuleFor(x => x.Password).Length(
                min: GlobalVariables.UserConstants.PasswordMinLength,
                max: GlobalVariables.UserConstants.PasswordMinLength)
                .WithMessage(UserError.InvalidPasswordLength.Description);
        }
    }
}
