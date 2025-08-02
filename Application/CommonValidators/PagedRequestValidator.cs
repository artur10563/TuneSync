using Application.DTOs;
using Domain.Errors;
using FluentValidation;

namespace Application.CommonValidators;

public abstract class PagedRequestValidator<T> : AbstractValidator<T> where T : IPaged
{
    protected PagedRequestValidator()
    {
        RuleFor(x => x.Page)
            .Must(page => page > 0)
            .WithMessage(PageError.InvalidPage.Description);
    }
}