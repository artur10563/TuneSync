using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Application.Services;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Common;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Application.DTOs.Songs;
using YoutubeVideo = Google.Apis.YouTube.v3.Data.Video;
using YoutubeThumbnail = Google.Apis.YouTube.v3.Data.Thumbnail;
using ExplodeVideo = YoutubeExplode.Videos.Video;
using ExplodeThumbnail = YoutubeExplode.Common.Thumbnail;
using System.Net;
using System.Text.Json;
using System.Web;
using Application.DTOs.Youtube;
using Domain.Helpers;
using Author = Application.DTOs.Youtube.Author;


namespace Infrastructure.Services
{
    public class YoutubeService : IYoutubeService
    {
        private readonly YouTubeService _service;
        private readonly YoutubeClient _youtubeExplode;
        private ILoggerService _logger;

        private static string _musicCategoryId = "10";
        
        public YoutubeService(IConfiguration configuration, ILoggerService logger)
        {
            _logger = logger;
            _service = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = configuration["YouTubeApi:ApiKey"],
                ApplicationName = this.GetType().ToString()
            });

            _youtubeExplode = new YoutubeClient();
        }


        private const int MaxYoutubeSearchResults = 20;


        public async Task<IEnumerable<YoutubeSongInfo>> SongsByPlaylistId(string playlistId, int maxResults)
        {
            if (maxResults > MaxYoutubeSearchResults) maxResults = MaxYoutubeSearchResults;
            
            var playlistItemsRequest = _service.PlaylistItems.List("snippet");
            playlistItemsRequest.PlaylistId = playlistId;
            playlistItemsRequest.MaxResults = maxResults;

            var playlistItemsResponse = await playlistItemsRequest.ExecuteAsync();
            
            return playlistItemsResponse.Items.Select(x =>
                {
                    var snippet = x.Snippet;
                    var thumbnail = snippet.Thumbnails.High;
                    
                    if (snippet.ResourceId.Kind == "youtube#video")
                    {
                        return new YoutubeSongInfo(
                            Id: snippet.ResourceId.VideoId,
                            Title: HttpUtility.HtmlDecode(snippet.Title),
                            Description: HttpUtility.HtmlDecode(snippet.Description),
                            Author: new SongAuthor(snippet.VideoOwnerChannelId, HttpUtility.HtmlDecode(snippet.VideoOwnerChannelTitle)),
                            Thumbnail: new SongThumbnail(
                                Height: (int)thumbnail?.Height,
                                Width: (int)thumbnail?.Width,
                                Url: thumbnail.Url
                            ), 
                            PlaylistId: snippet.PlaylistId
                        );
                    }

                    return null;
                })
                .Where(item => item != null)
                .ToList();
        }
        
        // Youtube explode takes 30 times longer to fetch video, so youtube api is used
        public async Task<IEnumerable<YoutubeSongInfo>> SearchAsync(string query, int maxResults = 5)
        {
            if (maxResults > MaxYoutubeSearchResults) maxResults = MaxYoutubeSearchResults;

            var searchListRequest = _service.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = maxResults;
            searchListRequest.Type = "video";
            searchListRequest.VideoCategoryId = _musicCategoryId;
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
            playlistSearchRequest.VideoCategoryId = _musicCategoryId;
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

        public async Task<AlbumInfo> GetPlaylistInfoAsync(string playlistId)
        {
            var playlistInfo = await _youtubeExplode.Playlists.GetAsync(playlistId);
            var thumbnail = playlistInfo.Thumbnails.FirstOrDefault();
            
            var playlistThumbnailId = YoutubeHelper.IsYoutubeMusic(playlistId) 
                ? thumbnail?.Url 
                : YoutubeHelper.GetPlaylistThumbnailIdFromUrl(thumbnail?.Url, playlistId);
            
            var albumThumbnail = new SongThumbnail(thumbnail?.Resolution.Height ?? 0, thumbnail?.Resolution.Width ?? 0, playlistThumbnailId);
            return new AlbumInfo(playlistInfo.Id, playlistInfo.Title, playlistInfo.Url, albumThumbnail);
        }
        

        public async Task<(List<YoutubeSongInfo>, SongThumbnail)> GetPlaylistVideosAsync(string playlistId)
        {
            var videosResponse = await _youtubeExplode.Playlists.GetVideosAsync(playlistId);
            var playlistInfo = await GetPlaylistInfoAsync(playlistId);
            
            var songs = videosResponse.Select(x => new YoutubeSongInfo(
                Id: x.Id.Value,
                Title: x.Title,
                Author: new SongAuthor(x.Author.ChannelId, x.Author.ChannelTitle),
                Description: playlistInfo.Title //Playlist name
            )).ToList();

            return (songs, playlistInfo.Thumbnail);
        }
        
        // DLP
        private ProcessStartInfo CreateProcessStartInfo(
            string arguments,
            bool redirectStandardOutput = true,
            bool redirectStandardError = true,
            bool useShellExecute = false,
            bool createNoWindow = true)
        {
            return new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = arguments,
                RedirectStandardOutput = redirectStandardOutput,
                RedirectStandardError = redirectStandardError,
                UseShellExecute = useShellExecute,
                CreateNoWindow = createNoWindow
            };
        }

        private async Task<string> RunYtDlpAsync(string arguments)
        {
            var processStartInfo = CreateProcessStartInfo(arguments);

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (!string.IsNullOrEmpty(error) && string.IsNullOrEmpty(output))
                {
                    throw new Exception($"yt-dlp error: {error}"); //if output present and we have error - just ignore it xd need to refactor this shit later and log
                }

                if (string.IsNullOrEmpty(output))
                {
                    throw new Exception("yt-dlp did not return any output.");
                }

                return output;
            }
        }

        public async Task<YouTubeVideoInfo> GetVideoInfoAsyncDLP(string videoId)
        {
            string jsonOutput = await RunYtDlpAsync($"--verbose -j https://www.youtube.com/watch?v={videoId}");

            using (JsonDocument doc = JsonDocument.Parse(jsonOutput))
            {
                JsonElement root = doc.RootElement;

                var v = new YouTubeVideoInfo
                {
                    VideoId = root.GetProperty("id").GetString(),
                    Title = root.GetProperty("title").GetString(),
                    Duration = TimeSpan.FromSeconds(root.GetProperty("duration").GetDouble()),
                    Author = new Author()
                    {
                        ChannelId = root.GetProperty("channel_id").GetString(),
                        ChannelTitle = root.GetProperty("uploader").GetString()
                    }
                };
                return v;
            }
        }

        public async Task<Stream> GetAudioStreamAsyncDLP(string videoId)
        {
            var memoryStream = new MemoryStream();

            var processStartInfo = CreateProcessStartInfo($"--verbose -f bestaudio -o - https://www.youtube.com/watch?v={videoId}");

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();

                // Copy to the memory stream
                await process.StandardOutput.BaseStream.CopyToAsync(memoryStream);
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    string error = await process.StandardError.ReadToEndAsync();
                    throw new Exception($"yt-dlp failed: {error}");
                }
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}