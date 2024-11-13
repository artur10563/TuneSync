using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Command.CreateSongFromYouTube
{
    public sealed record CreateSongFromYoutubeCommand(string SongName, string Author, string Url) : IRequest<Result<SongDTO>>
    {
        public static CreateSongFromYoutubeCommand Create(CreateSongFromYoutubeCommand unFiltered)
        {
            var decodedUrl = System.Web.HttpUtility.UrlDecode(unFiltered.Url);
            return new CreateSongFromYoutubeCommand(unFiltered.SongName, unFiltered.Author, decodedUrl);
        }
    }
}
