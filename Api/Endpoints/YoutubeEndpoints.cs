using Api.Extensions;
using Application.CQ.Playlists.Command.CreatePlaylistFromYoutube;
using Application.CQ.Playlists.Query.GetYoutubePlaylistId;
using Application.CQ.Songs.Command.CreateSongFromYouTube;
using Application.Repositories.Shared;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints;

public static class YoutubeEndpoints
{
    public static async Task RegisterYoutubeEndpoints(this IEndpointRouteBuilder app)
    {
        var ytGroup = app.MapGroup("api/youtube").WithTags("Youtube");
        var songGroup = ytGroup.MapGroup("/song");
        var playlistGroup = ytGroup.MapGroup("/playlist");
        
        songGroup.MapGet("/{query}", async (IYoutubeService _youtube, string query) =>
        {
            var result = (await _youtube.SearchAsync(query)).ToList();

            return result;
        }).WithDescription("Search song on youtube");
        
        
        songGroup.MapPost("/{videoLink}", async (string videoLink,
            ISender _sender, HttpContext _httpContext, IUnitOfWork _uow
        ) =>
        {
            var uid = _httpContext.GetExternalUserId();
            var user = await _uow.UserRepository.GetByExternalIdAsync(uid);

            var command = new CreateSongFromYoutubeCommand(videoLink.DecodeUrl(), user.Guid);
            var result = await _sender.Send(command);

            if (result.IsFailure)
                return Results.BadRequest(result.Errors);
            return Results.Created($"api/song/youtube/{result.Value}", result.Value);
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

        playlistGroup.MapPost("/{playlistId}", async (ISender _sender, HttpContext _httpContext, IUnitOfWork _uow, string playlistId) =>
        {
            var uid = _httpContext.GetExternalUserId();
            var user = await _uow.UserRepository.GetByExternalIdAsync(uid);

            var command = new CreatePlaylistFromYoutubeCommand(playlistId, user.Guid);
            var result = await _sender.Send(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.AcceptedAtRoute("DownloadingProgress", routeValues: new { jobId = result.Value }, value: result.Value);
        }).RequireAuthorization();

        app.MapGet("jobs/{jobId}",
                async (ISender _sender, IBackgroundJobService _backgroundService, string jobId) =>
                {
                    return Results.Ok(_backgroundService.GetJobDetails(jobId));
                })
            .WithName("DownloadingProgress")
            .RequireAuthorization();

    }
}