namespace Application.Services
{
	public record VideoInfo(string Title, string Author, string VideoId, string VideoUrl);

	public interface IYoutubeService
	{
		Task<IEnumerable<VideoInfo>> SearchAsync(string query);
	}
}
