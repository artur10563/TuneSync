using Application.DTOs.Songs;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Application.Services
{
    public record VideoInfo(string Title, string Author, string VideoId, string AudioUrl);

    public interface IYoutubeService
    {
        Task<IEnumerable<YoutubeSongInfo>> SearchAsync(string query, int maxResults = 5);
        Task<Stream> GetAudioStreamAsync(IStreamInfo streamInfo);
        Task<(Video videoInfo, IStreamInfo streamInfo)> GetVideoInfoAsync(string url);
        string GetVideoIdFromUrl(string url);
        Task<string> SearchPlaylistBySongAndAuthorAsync(string authorId, string songTitle);

        /// <returns>List of videos for specified playlist and thumbnailId for playlist</returns>
        Task<(List<YoutubeSongInfo>, string)> GetPlaylistVideosAsync(string playlistId);

        Task<(string title, Stream videoStream)> GetVideoAsync(string url);
    }
}