using Application.Repositories.Shared;
using Application.Services;
using Domain.Primitives;

namespace Api.Endpoints;

public static class AdminEndpoints
{
    /// <summary>
    /// Mostly util methods to bulk update broken data
    /// </summary>
    public static IEndpointRouteBuilder RegisterAdminEndpoints(this IEndpointRouteBuilder app)
    {
        //TODO: add role policy.
        var group = app.MapGroup("api/admin").WithTags("Admin").RequireAuthorization(policy => policy.RequireRole(GlobalVariables.UserConstants.Roles.Admin));
        var utils = group.MapGroup("/utils");

        utils.MapPost("", async (IYoutubeService _youtube, IUnitOfWork _uow) =>
        {
            var channelsToCheck = _uow.ArtistRepository
                .Where(x => string.IsNullOrEmpty(x.ThumbnailUrl))
                .ToList();
            
            foreach (var channel in channelsToCheck)
            {
                var info = await _youtube.GetChannelInfoAsync(channel.YoutubeChannelId);
                if (info.Thumbnail == null) continue;

                channel.ThumbnailUrl = info.Thumbnail.Url;
                _uow.ArtistRepository.Update(channel);
            }

            var rows =await _uow.SaveChangesAsync();
            return Results.Ok($"Updated {rows} records");
        });

        return app;
    }
}