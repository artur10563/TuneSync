using Api.Extensions;
using Application.CQ.Playlists.Query.GetYoutubePlaylistId;
using Application.CQ.Songs.Command.CreateSongFromYouTube;
using Application.Repositories.Shared;
using Application.Services;
using MediatR;

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
            return Results.Created($"api/song/youtube/{result.Value.Guid}", result.Value);

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

    }
}