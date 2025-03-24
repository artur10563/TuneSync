using Microsoft.Extensions.Configuration;
using Application.Services;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;
using YoutubeExplode.Common;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Application.DTOs.Songs;
using Google.Apis.YouTube.v3.Data;
using YoutubeVideo = Google.Apis.YouTube.v3.Data.Video;
using YoutubeThumbnail = Google.Apis.YouTube.v3.Data.Thumbnail;
using ExplodeVideo = YoutubeExplode.Videos.Video;
using ExplodeThumbnail = YoutubeExplode.Common.Thumbnail;
using System.Net;
using System.Web;


namespace Infrastructure.Services
{
    public class YoutubeService : IYoutubeService
    {
        private readonly YouTubeService _service;
        private readonly YoutubeClient _youtubeExplode;

        public YoutubeService(IConfiguration configuration)
        {
            _service = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = configuration["YouTubeApi:ApiKey"],
                ApplicationName = this.GetType().ToString()
            });
            
            //add even more variables to appsettings development for now :)
            //normal solution - refresh the cookies automatically
            
            var cookieList = new List<Cookie>();

            var cookieValues = configuration.GetSection("YouTubeCookies").Get<Dictionary<string, string>>();
            if (cookieValues != null)
            {
                foreach (var (name, value) in cookieValues)
                {
                    cookieList.Add(new Cookie(name, value, "/", ".youtube.com"));
                }
            }

            _youtubeExplode = new YoutubeClient(cookieList.AsReadOnly());
        }


        private const int MaxYoutubeSearchResults = 20;


        // Youtube explode takes 30 times longer to fetch video, so youtube api is used
        public async Task<IEnumerable<YoutubeSongInfo>> SearchAsync(string query, int maxResults = 5)
        {
            if (maxResults > MaxYoutubeSearchResults) maxResults = MaxYoutubeSearchResults;

            var searchListRequest = _service.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = maxResults;
            searchListRequest.Type = "video";
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;

            var searchListResponse = await searchListRequest.ExecuteAsync();

            var result = searchListResponse.Items
                .Select(x =>
                {
                    var snippet = x.Snippet;
                    var thumbnail = snippet.Thumbnails.High;

                    if (x.Id.Kind == "youtube#video")
                    {
                        return new YoutubeSongInfo(
                            Id: x.Id.VideoId,
                            Title: HttpUtility.HtmlDecode(snippet.Title),
                            Description: HttpUtility.HtmlDecode(snippet.Description),
                            Author: new SongAuthor(snippet.ChannelId, HttpUtility.HtmlDecode(snippet.ChannelTitle)),
                            Thumbnail: new SongThumbnail(
                                Height: (int)thumbnail?.Height,
                                Width: (int)thumbnail?.Width,
                                Url: thumbnail.Url
                            )
                        );
                    }

                    return null;
                })
                .Where(item => item != null)
                .ToList();

            return result;
        }

        /// <summary>
        /// Get playlist of a song
        /// </summary>
        /// <param name="authorId">Id of youtube channel, where we search the song</param>
        /// <param name="songTitle">Title of song</param>
        /// <returns>Playlist id</returns>
        public async Task<string?> SearchPlaylistBySongAndAuthorAsync(string authorId, string songTitle)
        {
            var playlistSearchRequest = _service.Search.List("snippet");
            playlistSearchRequest.Q = songTitle;
            playlistSearchRequest.MaxResults = 1;
            playlistSearchRequest.Type = "playlist";
            playlistSearchRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            playlistSearchRequest.ChannelId = authorId;

            var playlistSearchResponse = await playlistSearchRequest.ExecuteAsync();
            var playlistId = playlistSearchResponse?.Items?.FirstOrDefault()?.Id?.PlaylistId;
            return playlistId;
        }

        public async Task<(ExplodeVideo videoInfo, IStreamInfo streamInfo)> GetVideoInfoAsync(string url)
        {
            ExplodeVideo videoInfo = await _youtubeExplode.Videos.GetAsync(url);

            IStreamInfo streamInfo = (await _youtubeExplode.Videos.Streams.GetManifestAsync(videoInfo.Url))
                .GetAudioOnlyStreams()
                .GetWithHighestBitrate();


            return (videoInfo, streamInfo);
        }
        
        public async Task<string> GetVideoInfoAsync1(string url)
        {
            ExplodeVideo videoInfo = await _youtubeExplode.Videos.GetAsync(url);
            return videoInfo.Url;
        }
        
        public async Task<IStreamInfo> GetStreamInfoAsync(string url)
        {

            IStreamInfo streamInfo = (await _youtubeExplode.Videos.Streams.GetManifestAsync(url))
                .GetAudioOnlyStreams()
                .GetWithHighestBitrate();

            return streamInfo;
        }
        
        public async Task<ChannelInfo> GetChannelInfoAsync(string channelId)
        {
            var v = await _youtubeExplode.Channels.GetAsync(channelId);
            
            SongThumbnail? channelThumbnail = null;
            if (v.Thumbnails.Count != 0)
            {
                var thumbnail = v.Thumbnails[0]; 
                channelThumbnail = new SongThumbnail(thumbnail.Resolution.Height, thumbnail.Resolution.Width, thumbnail.Url);
            }
            
            return new ChannelInfo(v.Id, v.Title, v.Url, channelThumbnail);
        }
        
        public async Task<Stream> GetAudioStreamAsync(IStreamInfo streamInfo)
        {
            var audioStream = await _youtubeExplode.Videos.Streams.GetAsync(streamInfo);
            return audioStream;
        }

        public string GetVideoIdFromUrl(string url)
        {
            const string videoParam = "v=";
            var index = url.IndexOf(videoParam);
            if (index == -1) return string.Empty;

            var videoId = url.Substring(index + videoParam.Length);
            var ampIndex = videoId.IndexOf('&');
            return ampIndex == -1 ? videoId : videoId.Substring(0, ampIndex);
        }


        public async Task<(List<YoutubeSongInfo>, string)> GetPlaylistVideosAsync(string playlistId)
        {
            var videosResponse = await _youtubeExplode.Playlists.GetVideosAsync(playlistId);
            var playlistInfo = await _youtubeExplode.Playlists.GetAsync(playlistId);
            
            var playlistThumbnailId = GetPlaylistIdFromUrl(playlistInfo.Thumbnails[0]?.Url);
            
            var songs =videosResponse.Select(x => new YoutubeSongInfo(
                Id: x.Id.Value,
                Title: x.Title,
                Author: new SongAuthor(x.Author.ChannelId, x.Author.ChannelTitle),
                Description: playlistInfo.Title //Playlist name
            )).ToList();
            
            return (songs, playlistThumbnailId);
        }

        private string GetPlaylistIdFromUrl(string? url)
        {
            var id = "";
            if (string.IsNullOrEmpty(url)) return id;
            
            var parts = url.Split('/');
            if (parts.Length > 4)
            {
                id = parts[4];
            }

            return id;
        }
    }
}