using Api.Extensions;
using Application.CQ.Album.Query.GetAlbumDetailsById;
using Application.CQ.Album.Query.GetAlbumSongsById;
using Application.CQ.Album.Query.GetRandomAlbums;
using Application.DTOs.Albums;
using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Api.Endpoints;

public static class AlbumEndpoints
{
    public static IEndpointRouteBuilder RegisterAlbumEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/album").WithTags("Album");


        group.MapGet("/{albumGuid:guid}", async (Guid albumGuid, HttpContext context, ISender sender) =>
        {
            var user = await context.GetCurrentUserAsync();

            var query = new GetAlbumDetailsByIdQuery(albumGuid, user?.Guid);

            var result = await sender.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound();
        }).Produces<AlbumDTO>();
        
        group.MapGet("/{albumGuid:guid}/songs", async (Guid albumGuid, HttpContext context, ISender sender, int page = 1) =>
        {
            var user = await context.GetCurrentUserAsync();

            var query = new GetAlbumSongsByIdQuery(albumGuid, user?.Guid, page);

            var result = await sender.Send(query);

            return result.IsFailure
                ? Results.BadRequest(result.Errors)
                : !result.Value.Any()
                    ? Results.NoContent()
                    : Results.Ok(result.ToPaginatedResponse());
        }).Produces<PaginatedResponse<IEnumerable<SongDTO>>>();

        group.MapGet("/random", async (HttpContext context, ISender _sender) =>
        {
            var user = await context.GetCurrentUserAsync();
            var query = new GetRandomAlbumsQuery(user?.Guid ?? Guid.Empty);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound();
        }).Produces<IEnumerable<AlbumSummaryDTO>>();

        return app;
    }
}