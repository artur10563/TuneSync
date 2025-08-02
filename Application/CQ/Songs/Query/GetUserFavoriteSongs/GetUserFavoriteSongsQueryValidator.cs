using Application.CommonValidators;
using Domain.Entities;
using Domain.Errors;
using FluentValidation;

namespace Application.CQ.Songs.Query.GetUserFavoriteSongs;

public class GetUserFavoriteSongsQueryValidator : PagedRequestValidator<GetUserFavoriteSongsQuery>
{
    public GetUserFavoriteSongsQueryValidator()
    {
        RuleFor(x => x.UserGuid)
            .Must(userGuid => userGuid != Guid.Empty)
            .WithMessage(x => Error.NotFound(nameof(Song)).Description);
    }
}