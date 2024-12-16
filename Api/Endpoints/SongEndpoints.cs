using Api.Extensions;
using Application.CQ.Songs.Command.CreateSong;
using Application.CQ.Songs.Query.GetSongFromDb;
using Application.Repositories.Shared;
using MediatR;
using Application.CQ.Songs.Command;
using Domain.Primitives;

namespace Api.Endpoints
{
    public static class SongEndpoints
    {
        public static async Task RegisterSongsEndpoints(this IEndpointRouteBuilder app)
        {
            var songGroup = app.MapGroup("api/song").WithTags("Song");
            var favSongGroup = app.MapGroup("api/favorite/song").WithTags("Song");

            songGroup.MapGet("/{query}", async (ISender _sender, HttpContext _httpContext, string query) =>
            {
                var user = await _httpContext.GetCurrentUserAsync();

                var command = new GetSongFromDbCommand(query, user?.Guid);
                var result = await _sender.Send(command);

                return result.IsFailure
                    ? Results.BadRequest(result.Errors)
                    : result.Value.Count == 0
                        ? Results.NoContent()
                        : Results.Ok(result.Value);
            }).WithName("GetSong").WithDescription("Search from database");

            songGroup.MapPost("", async (
                IFormFile audioFile,
                Guid artistGuid,
                ISender sender,
                HttpContext _httpContext
            ) =>
            {
                var user = await _httpContext.GetCurrentUserAsync();

                using var stream = audioFile.OpenReadStream();
                var command = new CreateSongCommand(audioFile.FileName, artistGuid, stream, user!.Guid);
                var result = await sender.Send(command);

                return result.IsFailure
                    ? Results.BadRequest(result.Errors)
                    : Results.CreatedAtRoute(routeName: "GetSong", routeValues: new { query = result.Value.Guid.ToString() }, value: result.Value);
            }).DisableAntiforgery().RequireAuthorization().WithDescription("Upload from file"); //TODO: Add Antiforgery

            favSongGroup.MapPut("/{songGuid:guid}", async (ISender _sender, HttpContext _httpContext,
                Guid songGuid) =>
            {
                var user = await _httpContext.GetCurrentUserAsync();

                var command = new ToggleFavoriteSongCommand(songGuid, user!.Guid);
                var result = await _sender.Send(command);

                return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Errors);
            }).RequireAuthorization().WithDescription("Add/Remove song from favorites");
        }
    }
}