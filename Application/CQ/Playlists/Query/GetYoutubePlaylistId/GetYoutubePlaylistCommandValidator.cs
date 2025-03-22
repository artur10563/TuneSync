using Domain.Errors;
using Domain.Primitives;
using FluentValidation;

namespace Application.CQ.Playlists.Query.GetYoutubePlaylistId;

public class GetYoutubePlaylistCommandValidator : AbstractValidator<GetYoutubePlaylistCommand>
{
    public GetYoutubePlaylistCommandValidator()
    {
        RuleFor(x => x.SongTitle)
            .NotEmpty()
            .Length(
                min: GlobalVariables.SongConstants.TitleMinLength,
                max: GlobalVariables.SongConstants.TitleMaxLength)
            .WithMessage(SongError.InvalidTitleLength.Description);

        RuleFor(x => x.ChannelId)
            .NotEmpty()
            .Length(24) // max youtube channel Id`s are 24 symbols long
            .WithMessage("Bad channel id");
    }
}