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
            _youtubeExplode = new YoutubeClient();
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

            var searchListResponse = await searchListRequest.ExecuteAsync();

            var result = searchListResponse.Items
                .Select(x =>
                {
                    SearchResultSnippet snippet = x.Snippet;
                    YoutubeThumbnail thumbnail = snippet.Thumbnails.High;

                    if (x.Id.Kind == "youtube#video")
                    {
                        return new YoutubeSongInfo(
                            Id: x.Id.VideoId,
                            Title: snippet.Title,
                            Description: snippet.Description,
                            Author: new SongAuthor(snippet.ChannelId, snippet.ChannelTitle),
                            Thumbnail:
                            new SongThumbnail(
                                Height: (int)thumbnail?.Height,
                                Width: (int)thumbnail?.Width,
                                Url: thumbnail.Url
                                )
                            );
                    }
                    return null;
                }).ToList();

            return result;
        }


        public async Task<(ExplodeVideo videoInfo, IStreamInfo streamInfo)> GetVideoInfoAsync(string url)
        {
            ExplodeVideo videoInfo = await _youtubeExplode.Videos.GetAsync(url);

            IStreamInfo streamInfo = (await _youtubeExplode.Videos.Streams.GetManifestAsync(videoInfo.Url))
                .GetAudioOnlyStreams()
                .GetWithHighestBitrate();


            return (videoInfo, streamInfo);
        }

        public async Task<Stream> GetAudioStreamAsync(string url)
        {
            var audioInfo = (await _youtubeExplode.Videos.Streams.GetManifestAsync(url))
                .GetAudioOnlyStreams()
                .GetWithHighestBitrate();

            var audioStream = await _youtubeExplode.Videos.Streams.GetAsync(audioInfo);
            return audioStream;
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

    }
}
