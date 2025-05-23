﻿using Application.Repositories.Shared;
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
                .Length(11)
                .WithMessage("Invalid video ID");
                // .Matches(@"^(https?:\/\/)?(www\.)?youtube\.com\/watch\?v=([a-zA-Z0-9_-]{11})(&.*)?$")
                // .WithMessage("Invalid YouTube URL format. Please provide a valid YouTube watch URL");

            RuleFor(x => x.Url)
                .MustAsync(async (videoId, cancellationToken) =>
                {
                    var exists = _uow.SongRepository
                    .Where(x => x.Source == GlobalVariables.SongSource.YouTube && x.SourceId == videoId)
                    .Any();

                    return !exists;
                }).WithMessage(SongError.Exists.Description);

        }
    }
}
