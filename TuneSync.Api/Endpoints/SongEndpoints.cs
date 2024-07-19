using TuneSync.Application.Services;

namespace TuneSync.Api.Endpoints
{
	public static class SongEndpoints
	{
		public static void RegisterSongsEndpoints(this IEndpointRouteBuilder app)
		{
			app.MapGet("api/songs/{query}", async (string query, IYoutubeService _youtube) =>
			{
				var result = await _youtube.FindVideoAsync(query);
				return Results.Ok(result);
			});
		}

	}
}


