using Domain.Errors;
using FluentValidation;

namespace Application.CQ.Playlists.Query.GetPlaylistSongsById;

public class GetPlaylistSongsByIdCommandValidator : AbstractValidator<GetPlaylistSongsByIdCommand>
{
    public GetPlaylistSongsByIdCommandValidator()
    {
        RuleFor(x => x.Page)
            .Must(page => page > 0)
            .WithMessage(PageError.InvalidPage.Description);
    }
}

