using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Command.CreateSongFromYouTube
{
    public sealed record CreateSongFromYoutubeCommand(string Url, Guid CurrentUserGuid) : IRequest<Result<SongDTO>>;
}
