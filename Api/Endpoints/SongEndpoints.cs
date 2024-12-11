using Api.Extensions;
using Application.CQ.Songs.Command.CreateSong;
using Application.CQ.Songs.Command.CreateSongFromYouTube;
using Application.CQ.Songs.Query.GetSongFromDb;
using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Application.Services;
using MediatR;
using System.Web;
using Application.CQ.Playlists.Query.GetYoutubePlaylistId;

namespace Api.Endpoints
{
    public static class SongEndpoints
    {
        public static async Task RegisterSongsEndpoints(this IEndpointRouteBuilder app)
        {
            var songGroup = app.MapGroup("api/song").WithTags("Song");

            songGroup.MapGet("/{query}", async (ISender _sender, string query) =>
            {
                var command = new GetSongFromDbCommand(query);
                var result = await _sender.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);

                return Results.Ok(result.Value);
            }).WithDescription("Search from database");

            songGroup.MapPost("", async (
                IFormFile audioFile,
                Guid artistGuid,
                ISender sender,
                HttpContext _httpContext,
                IUnitOfWork _uow) =>
            {
                var uid = _httpContext.GetExternalUserId();
                var user = await _uow.UserRepository.GetByExternalIdAsync(uid);
                
                using var stream = audioFile.OpenReadStream();
                var command = new CreateSongCommand(audioFile.FileName, artistGuid, stream, user.Guid);
                var result = await sender.Send(command);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Errors);
                }
                return TypedResults.Created($"api/song/youtube/{result.Value.Guid}", result.Value);

            }).DisableAntiforgery().RequireAuthorization().WithDescription("Upload from file"); ; //TODO: Add Antiforgery
        }
    }
}