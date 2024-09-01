using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Google.Apis.YouTube.v3.Data;
using Application.Services;




namespace Infrastructure.Services
{
	public class YoutubeService : IYoutubeService
	{
		private readonly YouTubeService _service;

		public YoutubeService(IConfiguration configuration)
		{
			_service = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = configuration["YouTubeApi:ApiKey"],
				ApplicationName = this.GetType().ToString()
			});
		}


		public async Task<IEnumerable<VideoInfo>> SearchAsync(string query)
		{
			var searchListRequest = _service.Search.List("snippet");
			searchListRequest.Q = query;
			searchListRequest.MaxResults = 5;

			//items => videos
			//items[i].Id.VideoId
			//items[i].Snippet. ChannelId, ChannelTitle(Author), Title(SongName), Thumbnails(big, medium,small images URL)
			var searchListResponse = await searchListRequest.ExecuteAsync();

			var result = searchListResponse.Items
				.Where(item => !string.IsNullOrEmpty(item.Id.VideoId))
				.Select(item => new VideoInfo(
					item.Snippet.Title, item.Snippet.ChannelTitle, item.Id.VideoId, item.ETag)
				);

			return result;
		}
	}
}
