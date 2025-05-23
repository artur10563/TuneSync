﻿using Application.DTOs.Songs;
using Application.DTOs.Youtube;
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

        Task<IEnumerable<YoutubeSongInfo>> SongsByPlaylistId(string playlistId, int maxResults);
        
        /// <returns>List of videos for specified playlist and thumbnailId for playlist</returns>
        Task<(List<YoutubeSongInfo>, SongThumbnail)> GetPlaylistVideosAsync(string playlistId);

        Task<ChannelInfo> GetChannelInfoAsync(string channelId);
        

        Task<YouTubeVideoInfo> GetVideoInfoAsyncDLP(string videoId);
        Task<Stream> GetAudioStreamAsyncDLP(string videoId);

        /// <returns>Full url for YTM and only ID for others</returns>
        Task<AlbumInfo> GetPlaylistInfoAsync(string playlistId);
    }
}