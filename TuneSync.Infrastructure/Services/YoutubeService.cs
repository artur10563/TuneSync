using System.Threading.Tasks;
using TuneSync.Application.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Google.Apis.YouTube.v3.Data;

namespace TuneSync.Infrastructure.Services
{
	public class YoutubeService : IYoutubeService
	{
		private readonly string _apiKey;

		public YoutubeService(IConfiguration configuration)
		{
			_apiKey = configuration["YouTubeApi:ApiKey"];
		}


		public async Task<SearchListResponse> FindVideoAsync(string query)
		{

			var youtubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = _apiKey,
				ApplicationName = this.GetType().ToString()
			});

			var searchListRequest = youtubeService.Search.List("snippet");
			searchListRequest.Q = query;
			searchListRequest.MaxResults = 5;


			var searchListResponse = await searchListRequest.ExecuteAsync();
			return searchListResponse;
		}
	}
}

