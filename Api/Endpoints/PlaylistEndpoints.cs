using Api.Extensions;
using Application.CQ.Playlists.Command.Create;
using Application.CQ.Playlists.Command.DeletePlaylist;
using Application.CQ.Playlists.Command.DeleteSongFromPlaylist;
using Application.CQ.Playlists.Query.GetById;
using Application.CQ.Playlists.Query.GetPlaylistsByUser;
using Application.CQ.Playlists.Query.GetPlaylistSongsById;
using Application.DTOs.Playlists;
using MediatR;

namespace Api.Endpoints
{
    public static class PlaylistEndpoints
    {
        public static IEndpointRouteBuilder RegisterPlaylistEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/playlist").WithTags("Playlist");

            //Create new playlist
            group.MapPost("", async (ISender sender, HttpContext _httpContext,
                    string playlistTitle) =>
                {
                    var user = await _httpContext.GetCurrentUserAsync();

                    var command = new CreatePlaylistCommand(playlistTitle, user!.Guid);

                    var result = await sender.Send(command);

                    return result.IsFailure ? Results.BadRequest(result.Errors) : Results.Created($"api/playlist/{result.Value}", result.Value);
                })
                .RequireAuthorization()
                .WithDescription("Create new playlist").Produces<Guid>();

            group.MapGet("/{guid}", async (ISender sender, HttpContext _httpContext, Guid guid) =>
                {
                    var user = await _httpContext.GetCurrentUserAsync();

                    var command = new GetPlaylistDetailsByIdCommand(guid, user?.Guid);
                    var result = await sender.Send(command);
                    return result.IsFailure ? Results.BadRequest(result.Errors) : Results.Ok(result.Value);
                })
                .WithDescription("Get playlist details by Guid").Produces<PlaylistDTO>();

            group.MapGet("/{guid}/songs", async (ISender sender, HttpContext _httpContext, Guid guid, int page) =>
            {
                var user = await _httpContext.GetCurrentUserAsync();
                
                var command = new GetPlaylistSongsByIdCommand(user?.Guid, guid, page);
                var result = await sender.Send(command);
                return result.IsFailure
                    ? Results.BadRequest(result.Errors)
                    : !result.Value.Any()
                        ? Results.NoContent()
                        : Results.Ok(result.ToPaginatedResponse());
            });//.RequireAuthorization();

            group.MapPost("/{playlistGuid}/songs/{songGuid}", async (ISender sender, HttpContext _httpContext,
                    Guid playlistGuid,
                    Guid songGuid) =>
                {
                    var user = await _httpContext.GetCurrentUserAsync();

                    var command = new AddSongToPlaylistCommand(playlistGuid, songGuid, user!.Guid);
                    var result = await sender.Send(command);

                    return result.IsFailure ? Results.BadRequest(result.Errors) : Results.Created($"api/playlist/{playlistGuid}", playlistGuid);
                })
                .RequireAuthorization()
                .WithDescription("Add song to playlist");

            group.MapGet("", async (ISender sender, HttpContext _httpContext) =>
                {
                    var user = await _httpContext.GetCurrentUserAsync();

                    var command = new GetPlaylistsByUserCommand(user!.Guid);
                    var result = await sender.Send(command);

                    return result.IsFailure ? Results.BadRequest(result.Errors) : Results.Ok(result.Value);
                })
                .RequireAuthorization()
                .WithDescription("Current user playlists (without songs)")
                .Produces<IEnumerable<PlaylistSummaryDTO>>();

            group.MapDelete("/{playlistGuid:guid}/songs/{songGuid:guid}",
                    async (ISender sender, HttpContext _httpContext,
                        Guid playlistGuid,
                        Guid songGuid) =>
                    {
                        var user = await _httpContext.GetCurrentUserAsync();

                        var command = new DeleteSongFromPlaylistCommand(playlistGuid, songGuid, user!.Guid);
                        var result = await sender.Send(command);
                        return result.IsFailure ? Results.BadRequest(result.Errors) : Results.NoContent();
                    }).RequireAuthorization()
                .WithDescription("Deletes song from playlist")
                .WithName("DeleteSongFromPlaylist");

            group.MapDelete("/{playlistGuid:guid}", async (ISender sender, HttpContext _httpContext, Guid playlistGuid) =>
            {
                var user = await _httpContext.GetCurrentUserAsync();

                var command = new DeletePlaylistCommand(playlistGuid, user!.Guid);
                var result = await sender.Send(command);
                
                return result.IsFailure ? Results.BadRequest(result.Errors) : Results.NoContent();
            }).RequireAuthorization()
                .WithDescription("Delete playlist")
                .WithName("DeletePlaylist");

            return app;
        }
    }
}