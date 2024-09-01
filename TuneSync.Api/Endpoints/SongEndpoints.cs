using TuneSync.Application.Repositories;
using TuneSync.Application.Services;
using TuneSync.Domain.Entities;

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


			app.MapPost("api/songs", async (ISongRepository _songs, Song newSong) =>
			{
				for (int i = 0; i < 10_000; i++)
					await _songs.Create(newSong);
				return Results.Created();
			});

			app.MapGet("api/songs-firebase/", async (ISongRepository _songs) =>
			{
				return Results.Ok(await _songs.GetAsync());
			});

			app.MapGet("api/songs-firebase/{query}", async (string query, ISongRepository _songs) =>
			{

				//Class queryConstructor FieldName, operation, CompareTo.  Downside: clean architecture rip 
				//_songs.Get(new queryFilter("Field", Operation.Equal, "Value"));
				//songs.Get(new QueryFilter(nameof(x.Name), Operation.Equal), query)
				////Find by song name
				///
				
				var filters = new List<QueryFilter>
				{
					new(nameof(Song.Name), "==", "qwerts", QueryComparison.And),
					new(nameof(Song.VideoUrl), "Contains", ".com", QueryComparison.Or),
					new("TestNumber", ">", 0),
				};

				//_songs.GetAsync(filters);
				var r = await _songs.GetAsync(filters);

				return Results.Ok(r);
			});
		}

	}
	//Add next operation instruction? Next: OR, AND, etc
	//record QueryFilter(string Field, string Operation, object? Value, string? NextComparison = null);
}

//Search for a song:
// Firstly search over firebase database. 
// If not found, search over youtube, show first N videos, let user download them to firebase
