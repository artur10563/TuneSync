using Api.Extensions;
using Application.CQ.Playlists;
using Application.CQ.Playlists.Command.Create;
using Application.CQ.Playlists.Query.GetById;
using Application.Repositories.Shared;
using MediatR;

namespace Api.Endpoints
{
    public static class PlaylistEndpoints
    {
        public static async Task RegisterPlaylistEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/playlist").WithTags("Playlist");

            //Create new playlist
            group.MapPost("", async (ISender sender, HttpContext _httpContext, IUnitOfWork _uow,
                string playlistTitle) =>
            {
                var userId = _httpContext.GetExternalUserId();
                var user = await _uow.UserRepository.GetByExternalIdAsync(userId);

                var command = new CreatePlaylistCommand(playlistTitle, user!.Guid);

                var result = await sender.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);
                return Results.Created($"api/playlist/{result.Value}", result.Value);
            }).RequireAuthorization();

            //Get playlist by Guid
            group.MapGet("/{guid}", async (ISender sender, Guid guid) =>
            {
                var command = new GetPlaylistByIdCommand(guid);
                var result = await sender.Send(command);
                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);
                return Results.Created($"api/playlist/{result.Value}", result.Value);
            });

            //Add song to playlist
            group.MapPost("/{playlistGuid}/songs/{songGuid}", async (ISender sender, HttpContext _http, IUnitOfWork _uow,
                Guid playlistGuid,
                Guid songGuid) =>
            {
                var uid = _http.GetExternalUserId();
                var user = await _uow.UserRepository.GetByExternalIdAsync(uid);

                var command = new AddSongToPlaylistCommand(playlistGuid, songGuid, user.Guid);
                var result = await sender.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);

                return Results.Created($"api/playlist/{playlistGuid}", playlistGuid);
            });
        
        }
    }
}
