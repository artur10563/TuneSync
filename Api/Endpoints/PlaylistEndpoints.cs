using Api.Extensions;
using Application.CQ.Playlists.Command.Create;
using Application.CQ.Playlists.Command.DeleteSongFromPlaylist;
using Application.CQ.Playlists.Query.GetById;
using Application.CQ.Playlists.Query.GetPlaylistsByUser;
using MediatR;

namespace Api.Endpoints
{
    public static class PlaylistEndpoints
    {
        public static async Task RegisterPlaylistEndpoints(this IEndpointRouteBuilder app)
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
                .WithDescription("Create new playlist");

            group.MapGet("/{guid}", async (ISender sender, HttpContext _httpContext, Guid guid) =>
                {
                    var user = await _httpContext.GetCurrentUserAsync();
                    
                    var command = new GetPlaylistByIdCommand(guid, user?.Guid);
                    var result = await sender.Send(command);
                    return result.IsFailure ? Results.BadRequest(result.Errors) : Results.Ok(result.Value);
                })
                .WithDescription("Get playlist with songs by Guid");

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
                .WithDescription("Current user playlists (without songs)");

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
        }
    }
}