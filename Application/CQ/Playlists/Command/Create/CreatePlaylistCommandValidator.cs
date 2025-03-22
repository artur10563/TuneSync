using Domain.Errors;
using Domain.Primitives;
using FluentValidation;

namespace Application.CQ.Playlists.Command.Create
{
    public sealed class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand>
    {
        public CreatePlaylistCommandValidator()
        {
            RuleFor(x => x.PlaylistName).Length(
                min: GlobalVariables.PlaylistConstants.TitleMinLength,
                max: GlobalVariables.PlaylistConstants.TitleMaxLength)
                .WithMessage(x => PlaylistError.InvalidTitleLength.Description);

            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage(PlaylistError.OwnerRequired.Description);
        }
    }
}
