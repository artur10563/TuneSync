using Api.Extensions;
using Application.CQ.Album.Query.GetArtistList;
using Application.CQ.Artists.Query.GetArtistSummary;
using Application.DTOs.Artists;
using Domain.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using static Domain.Primitives.GlobalVariables;

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

        artistGroup.MapGet("", async (ISender sender, HttpContext httpContext,
            int page = PaginationConstants.PageMin,
            int pageSize = PaginationConstants.PageSize,
            string orderBy = "CreatedAt",
            bool descending = false) =>
        {
            var user = await httpContext.GetCurrentUserAsync();

            //Amount, page, order by
            var query = new GetArtistListQuery(page, pageSize, orderBy, descending);
            var result = await sender.Send(query);

            return result.IsFailure
                ? Results.BadRequest(result.Errors)
                : result.Value.Count() == 0
                    ? Results.NoContent()
                    : Results.Ok(result.ToPaginatedResponse());
        }).Produces<PaginatedResponse<List<ArtistInfoDTO>>>();

        return app;
    }
}