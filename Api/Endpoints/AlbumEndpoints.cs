using Api.Extensions;
using Application.CQ.Album.Query.GetAlbumById;
using MediatR;

namespace Api.Endpoints;

public static class AlbumEndpoints
{
    public static IEndpointRouteBuilder RegisterAlbumEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/albums").WithTags("albums");

        group.MapGet("/{albumGuid:guid}", async (Guid albumGuid, HttpContext context, ISender _sender) =>
        {
            var user = await context.GetCurrentUserAsync();

            var query = new GetAlbumByIdQuery(albumGuid, user?.Guid);

            var result = await _sender.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound();
        });


        return app;
    }
}