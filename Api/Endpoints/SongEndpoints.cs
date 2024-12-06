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
            var youtubeGroup = app.MapGroup("api/song/youtube").WithTags("Youtube");

            youtubeGroup.MapGet("/{query}", async (IYoutubeService _youtube, string query) =>
            {
                var result = (await _youtube.SearchAsync(query)).ToList();

                return result;
            }).WithDescription("Search on youtube");

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
                ISender sender) =>
            {
                using var stream = audioFile.OpenReadStream();
                var command = new CreateSongCommand(audioFile.FileName, artistGuid, stream);
                var result = await sender.Send(command);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Errors);
                }
                return TypedResults.Created($"api/song/youtube/{result.Value.Guid}", result.Value);

            }).DisableAntiforgery().RequireAuthorization().WithDescription("Upload from file"); ; //TODO: Add


            youtubeGroup.MapPost("/{videoLink}", async (string videoLink,
                ISender _sender, HttpContext _httpContext, IUnitOfWork _uow
                ) =>
            {
                var uid = _httpContext.GetExternalUserId();
                var user = await _uow.UserRepository.GetByExternalIdAsync(uid);

                var command = new CreateSongFromYoutubeCommand(videoLink.DecodeUrl(), user.Guid);
                var result = await _sender.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);
                return Results.Created($"api/song/youtube/{result.Value.Guid}", result.Value);

            }).RequireAuthorization().WithDescription("Upload from youtube by video url");

            youtubeGroup.MapGet("/{channelId}/{songTitle}",
                async (ISender _sender, string channelId, string songTitle) =>
                {
                    var command = new GetYoutubePlaylistCommand(channelId, songTitle);
                    var result = await _sender.Send(command);
                    
                    if (result.IsFailure)
                        return Results.BadRequest(result.Errors);
                    return Results.Ok(result.Value);
                    
                }).WithDescription("Find youtube song in playlist of specified channel");

        }
    }
}