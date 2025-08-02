using Api.Extensions;
using Application.CQ.Album.Command;
using Application.CQ.Album.Query.GetUserFavoriteAlbums;
using Application.CQ.Playlists.Command.ToggleFavoritePlaylist;
using Application.CQ.Playlists.Query.GetUserFavoritePlaylists;
using Application.CQ.Songs.Command;
using Application.CQ.Songs.Query.GetUserFavoriteSongs;
using Application.DTOs.Albums;
using Application.DTOs.Playlists;
using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Api.Endpoints;

public static class FavoriteEndpoints
{
    public static IEndpointRouteBuilder RegisterFavoriteEndpoints(this IEndpointRouteBuilder app)
    {
        var favGroup = app.MapGroup("api/favorite").RequireAuthorization().WithTags("Favorite");

        #region song
        
        favGroup.MapPut("/song/{songGuid:guid}", async (ISender _sender, HttpContext _httpContext,
            Guid songGuid) =>
        {
            var user = await _httpContext.GetCurrentUserAsync();

            var command = new ToggleFavoriteSongCommand(songGuid, user!.Guid);
            var result = await _sender.Send(command);

            return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Errors);
        }).WithDescription("Add/Remove song from favorites");

        favGroup.MapGet("/song", async (ISender _sender, HttpContext _httpContext, int page) =>
        {
            var user = await _httpContext.GetCurrentUserAsync();

            var command = new GetUserFavoriteSongsQuery(user!.Guid, page);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? !result.Value.Any()
                    ? Results.NoContent()
                    : Results.Ok(result.ToPaginatedResponse())
                : Results.BadRequest(result.Errors);
        }).WithDescription("Get favorite songs of current user").Produces<IEnumerable<PaginatedResponse<IEnumerable<SongDTO>>>>();
        
        #endregion

        #region album

        favGroup.MapPut("/album/{albumGuid:guid}", async (ISender _sender, HttpContext _httpContext,
            Guid albumGuid) =>
        {
            var user = await _httpContext.GetCurrentUserAsync();
        
            var command = new ToggleFavoriteAlbumCommand(albumGuid, user!.Guid);
            var result = await _sender.Send(command);
        
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Errors);
        }).WithDescription("Add/Remove album from favorites");

        favGroup.MapGet("/album", async (ISender _sender, HttpContext _httpContext) =>
        {
            var user = await _httpContext.GetCurrentUserAsync();

            var command = new GetUserFavoriteAlbumsQuery(user!.Guid);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? !result.Value.Any()
                    ? Results.NoContent()
                    : Results.Ok(result.Value)
                : Results.BadRequest(result.Errors);
        }).WithDescription("Get favorite albums of current user").Produces<IEnumerable<AlbumSummaryDTO>>();
        
        #endregion
        
        #region playlist

        favGroup.MapPut("/playlist/{playlistGuid:guid}", async (ISender _sender, HttpContext _httpContext,
            Guid playlistGuid) =>
        {
            var user = await _httpContext.GetCurrentUserAsync();
        
            var command = new ToggleFavoritePlaylistCommand(playlistGuid, user!.Guid);
            var result = await _sender.Send(command);
        
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Errors);
        }).WithDescription("Add/Remove playlist from favorites");

        favGroup.MapGet("/playlist", async (ISender _sender, HttpContext _httpContext) =>
        {
            var user = await _httpContext.GetCurrentUserAsync();

            var command = new GetUserFavoritePlaylistsQuery(user!.Guid);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? !result.Value.Any()
                    ? Results.NoContent()
                    : Results.Ok(result.Value)
                : Results.BadRequest(result.Errors);
        }).WithDescription("Get favorite playlists of current user").Produces<IEnumerable<PlaylistSummaryDTO>>();
        
        #endregion

        return app;
    }
}