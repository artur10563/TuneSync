using Api.Extensions;
using Application.CQ.Album.Query.GetArtistList;
using Application.CQ.Artists.Command.MergeArtists;
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
            string? query = null,
            bool collapseChildren = false,
            int page = PaginationConstants.PageMin,
            int pageSize = PaginationConstants.PageSize,
            string orderBy = "CreatedAt",
            bool descending = false) =>
        {
            var user = await httpContext.GetCurrentUserAsync(); // TODO: update it after favorite-artists is implemented

            //Amount, page, order by
            var command = new GetArtistListQuery(query, collapseChildren, page, pageSize, orderBy, descending);
            var result = await sender.Send(command);

            return result.IsFailure
                ? Results.BadRequest(result.Errors)
                : result.Value.Count() == 0
                    ? Results.NoContent()
                    : Results.Ok(result.ToPaginatedResponse());
        }).Produces<PaginatedResponse<List<ArtistInfoWithCountsDTO>>>();

        artistGroup.MapPost("/{parentId}/merge/{childId}", async (Guid parentId, Guid childId, ISender sender) =>
        {
            var command = new MergeArtistsCommand(parentId, childId);
            var result = await sender.Send(command);

            return result.IsFailure
                ? Results.BadRequest(result.Errors)
                : Results.Created();
            
        }).RequireAuthorization(policy=> policy.RequireRole(UserConstants.Roles.Admin));
            
        return app;
    }
}