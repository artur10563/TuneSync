﻿using Application.DTOs.Songs;
using Application.Helpers;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Command.CreateSongFromYouTube
{
    public sealed record CreateSongFromYoutubeCommand(string Url, Guid CurrentUserGuid) : IRequest<Result<Guid>>
    {
        public static CreateSongFromYoutubeCommand Create(string url, Guid currentUserGuid)
        {
            return new CreateSongFromYoutubeCommand(YoutubeHelpers.ExtractVideoIdFromUrl(url), currentUserGuid);
        }
    };
}
