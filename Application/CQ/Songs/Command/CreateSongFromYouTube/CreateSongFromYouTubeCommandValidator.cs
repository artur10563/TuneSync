using Application.Repositories.Shared;
using Application.Services;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;

namespace Application.CQ.Songs.Command.CreateSongFromYouTube
{
    public class CreateSongFromYouTubeCommandValidator : AbstractValidator<CreateSongFromYoutubeCommand>
    {
        public CreateSongFromYouTubeCommandValidator(IYoutubeService _youtube, IUnitOfWork _uow)
        {
            //youtube/watch?v=id
            RuleFor(x => x.Url)
                .NotEmpty()
                .Matches(@"^(https?:\/\/)?(www\.)?youtube\.com\/watch\?v=([a-zA-Z0-9_-]{11})(&.*)?$")
                .WithMessage("Invalid YouTube URL format. Please provide a valid YouTube watch URL");

            RuleFor(x => x.Url)
                .MustAsync(async (url, cancellationToken) =>
                {
                    var videoId = _youtube.GetVideoIdFromUrl(url);
                    var exists = _uow.SongRepository
                    .Where(x => x.Source == GlobalVariables.SongSource.YouTube && x.SourceId == videoId)
                    .Any();

                    return !exists;
                }).WithMessage(SongError.Exists.Description);

            RuleFor(x => x.Author).Length(
                min: GlobalVariables.SongConstants.TitleMinLength,
                max: GlobalVariables.SongConstants.TitleMaxLength);
            RuleFor(x => x.SongName).Length(
                min: GlobalVariables.SongConstants.TitleMinLength,
                max: GlobalVariables.SongConstants.TitleMaxLength);
        }
    }
}
