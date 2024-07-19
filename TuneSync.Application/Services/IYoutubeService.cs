using Google.Apis.YouTube.v3.Data;
using System.Threading.Tasks;

namespace TuneSync.Application.Services
{
	public interface IYoutubeService
	{
		Task<SearchListResponse> FindVideoAsync(string query); 
	}
}
