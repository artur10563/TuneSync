using Api.Extensions;
using Application.CQ.Artists.Query.GetArtistSummary;
using Application.DTOs.Artists;
using MediatR;

namespace Api.Endpoints;

public static class ArtistEndpoints
{
    public static IEndpointRouteBuilder RegisterArtistEndpoints(this IEndpointRouteBuilder app)
    {
        var artistGroup = app.MapGroup("api/artist").WithTags("Artist");
        
        //Return artist summary, artist albums and tracks without album
        artistGroup.MapGet("/{artistGuid:guid}", async (Guid artistGuid,
            HttpContext _httpContext,
            ISender _sender) =>
        {
            var user = await _httpContext.GetCurrentUserAsync();

            var query = new GetArtistSummaryQuery(artistGuid, user?.Guid ?? Guid.Empty);
            var result = await _sender.Send(query);

            return result.IsFailure
                ? Results.NotFound(result.Errors)
                : Results.Ok(result.Value);
        }).Produces<ArtistSummaryDTO>();

        return app;
    }
}