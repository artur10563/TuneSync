using Domain.Entities;
using Domain.Errors;
using FluentValidation;

namespace Application.CQ.Playlists.Command.CreatePlaylistFromYoutube;

public class CreatePlaylistFromYoutubeCommandValidator : AbstractValidator<CreatePlaylistFromYoutubeCommand>
{
    public CreatePlaylistFromYoutubeCommandValidator()
    {
        RuleFor(x => x.PlaylistId)
            .NotEmpty()
            .WithMessage("Bad playlist id");

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage(Error.Required(nameof(User)).Description);
    }
}