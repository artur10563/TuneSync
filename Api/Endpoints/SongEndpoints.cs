using Application.Repositories.Shared;
using Application.Services;
using Microsoft.AspNetCore.Http;

namespace Api.Endpoints
{
	public static class SongEndpoints
	{
		public static async Task RegisterSongsEndpoints(this IEndpointRouteBuilder app)
		{
			app.MapGet("api/song/youtube/{query}", async (
				IYoutubeService _youtube,
				IUnitOfWork _uow,
				string query) =>
			{
				var result = new List<VideoInfo>();


				result = _uow.SongRepository.Where(s => s.Title.Contains(query) || s.Artist.Contains(query))
					.Select(x => new VideoInfo(x.Title, "TestAuthor", "TestVideoId", "TestVideoUrl")).ToList();
				if (result.Count > 0) return result;

				result = (await _youtube.SearchAsync(query)).ToList();

				return result;
			});


			//Upload file from PC
			app.MapPost("api/song", async (IFormFile file, IFirebaseStorageService _fileStorage) =>
			{
				using var memoryStream = new MemoryStream();
				await file.CopyToAsync(memoryStream);
				memoryStream.Position = 0;
				await _fileStorage.UploadFile(memoryStream);

			}).DisableAntiforgery(); //TODO: Add
		}
	}
}

// Song found in database => return ready to play element
// Song not found in database => searech youtube, return 5 video results (title, author, youtube url for posibility of preview,).
// User can click a button to confirm the correct song and download it to our database
//User can add song from his pc

//Posiblle Endpoints:

//Download Song Endpoint:
// takes IFormFile / byte array and saves mp3 to firebase storage
// song search from youtube
// song search from database
// song search from other source

//Validation: 
// Max length of song OR YouTube - (Length of video * kb/sec)