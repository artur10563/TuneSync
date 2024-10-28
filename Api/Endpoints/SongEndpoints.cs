using Application.CQ.Songs.Command.CreateSong;
using Application.CQ.Songs.Command.CreateSongFromYouTube;
using Application.CQ.Songs.Query.GetSongFromDb;
using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Application.Services;
using MediatR;

namespace Api.Endpoints
{
    public static class SongEndpoints
    {
        public static async Task RegisterSongsEndpoints(this IEndpointRouteBuilder app)
        {
            //Search on youtube
            app.MapGet("api/song/youtube/{query}", async (IYoutubeService _youtube, string query) =>
            {
                var result = (await _youtube.SearchAsync(query)).ToList();

                return result;
            });

            //Mass search from database
            app.MapGet("api/song/{query}", async (ISender _sender, string query) =>
            {
                var command = new GetSongFromDbCommand(query);
                var result = await _sender.Send(command);

                if(result.IsFailure)
                    return Results.BadRequest(result.Error);

                return Results.Ok(result.Value);
            });

            //Upload from file
            app.MapPost("api/song", async (
                IFormFile audioFile,
                ISender sender) =>
            {
                using var stream = audioFile.OpenReadStream();
                var command = new CreateSongCommand(audioFile.FileName, stream);
                var result = await sender.Send(command);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return TypedResults.Created($"api/song/youtube/{result.Value.Guid}", result.Value);

            }).DisableAntiforgery(); //TODO: Add


            //Upload from youtube
            app.MapPost("api/song/youtube/{videoLink}", async (string videoLink,
                ISender _sender
                ) =>
            {
                var command = new CreateSongFromYoutubeCommand(videoLink);
                var result = await _sender.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result.Error);
                return Results.Created($"api/song/youtube/{result.Value.Guid}", result.Value);

            });
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