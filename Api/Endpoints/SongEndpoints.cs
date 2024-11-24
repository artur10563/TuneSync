using Api.Extensions;
using Application.CQ.Songs.Command.CreateSong;
using Application.CQ.Songs.Command.CreateSongFromYouTube;
using Application.CQ.Songs.Query.GetSongFromDb;
using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Application.Services;
using MediatR;
using System.Web;

namespace Api.Endpoints
{
    public static class SongEndpoints
    {
        public static async Task RegisterSongsEndpoints(this IEndpointRouteBuilder app)
        {
            var songGroup = app.MapGroup("api/song").WithTags("Song");
            var youtubeGroup = app.MapGroup("api/song/youtube").WithTags("Youtube");

            //Search on youtube
            youtubeGroup.MapGet("/{query}", async (IYoutubeService _youtube, string query) =>
            {
                var result = (await _youtube.SearchAsync(query)).ToList();

                return result;
            });

            //Mass search from database
            songGroup.MapGet("/{query}", async (ISender _sender, string query) =>
            {
                var command = new GetSongFromDbCommand(query);
                var result = await _sender.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);

                return Results.Ok(result.Value);
            });

            //Upload from file
            songGroup.MapPost("", async (
                IFormFile audioFile,
                ISender sender) =>
            {
                using var stream = audioFile.OpenReadStream();
                var command = new CreateSongCommand(audioFile.FileName, stream);
                var result = await sender.Send(command);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Errors);
                }
                return TypedResults.Created($"api/song/youtube/{result.Value.Guid}", result.Value);

            }).DisableAntiforgery().RequireAuthorization(); //TODO: Add


            //Upload from youtube
            youtubeGroup.MapPost("/{videoLink}", async (CreateSongFromYoutubeCommand request,
                ISender _sender
                ) =>
            {
                var result = await _sender.Send(CreateSongFromYoutubeCommand.Create(request));

                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);
                return Results.Created($"api/song/youtube/{result.Value.Guid}", result.Value);

            }).RequireAuthorization();
        }
    }
}