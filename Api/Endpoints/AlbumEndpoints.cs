using Api.Extensions;
using Application.CQ.Album.Query.GetAlbumById;
using Application.DTOs.Albums;
using MediatR;

namespace Api.Endpoints;

public static class AlbumEndpoints
{
    public static IEndpointRouteBuilder RegisterAlbumEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/album").WithTags("Album");


        group.MapGet("/{albumGuid:guid}", async (Guid albumGuid, HttpContext context, ISender _sender, int page = 1) =>
        {
            var user = await context.GetCurrentUserAsync();

            var query = new GetAlbumByIdQuery(albumGuid, user?.Guid, page);

            var result = await _sender.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound();
        }).Produces<AlbumDTO>();


        return app;
    }
}