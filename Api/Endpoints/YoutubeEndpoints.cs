using Api.Extensions;
using Application.CQ.Playlists.Command.CreatePlaylistFromYoutube;
using Application.CQ.Playlists.Query.GetYoutubePlaylistId;
using Application.CQ.Songs.Command.CreateSongFromYouTube;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints;

public static class YoutubeEndpoints
{
    public static IEndpointRouteBuilder RegisterYoutubeEndpoints(this IEndpointRouteBuilder app)
    {
        var ytGroup = app.MapGroup("api/youtube").WithTags("Youtube");
        var songGroup = ytGroup.MapGroup("/song");
        var playlistGroup = ytGroup.MapGroup("/playlist");

        songGroup.MapGet("/{query}", async (IYoutubeService _youtube, string query) =>
        {
            var result = (await _youtube.SearchAsync(query)).ToList();

            return result;
        }).WithDescription("Search song on youtube").RequireRateLimiting("fixed");


        songGroup.MapPost("/{videoLink}", async (string videoLink,
            ISender _sender, HttpContext _httpContext
        ) =>
        {
            var user = await _httpContext.GetCurrentUserAsync();

            var command = new CreateSongFromYoutubeCommand(videoLink.DecodeUrl(), user!.Guid);
            var result = await _sender.Send(command);

            return result.IsFailure
                ? Results.BadRequest(result.Errors)
                : Results.CreatedAtRoute(routeName: "GetSong", routeValues: new { query = result.Value.ToString() }, value: result.Value);
        }).RequireAuthorization().WithDescription("Upload from youtube by video url");


        playlistGroup.MapGet("/{channelId}/{songTitle}",
            async (ISender _sender, string channelId, string songTitle) =>
            {
                var command = new GetYoutubePlaylistCommand(channelId, songTitle);
                var result = await _sender.Send(command);

                if (result.IsFailure)
                    return Results.BadRequest(result.Errors);
                return Results.Ok(result.Value);
            }).WithDescription("Find playlist by song and specified channel");

        playlistGroup.MapPost("/{playlistId}", async (ISender _sender, HttpContext _httpContext, string playlistId) =>
        {
            var user = await _httpContext.GetCurrentUserAsync();

            var command = new CreatePlaylistFromYoutubeCommand(playlistId, user!.Guid);
            var result = await _sender.Send(command);
            return result.IsFailure 
                ? Results.BadRequest(result.Errors) 
                : Results.AcceptedAtRoute(
                    "DownloadingProgress",
                    routeValues: new { jobId = result.Value }, 
                    value: result.Value);
        }).RequireAuthorization();

        return app;
    }
}