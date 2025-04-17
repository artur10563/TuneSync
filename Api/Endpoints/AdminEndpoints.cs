using Application.BackgroundJobs;
using Application.CQ.Admin.Albums.Command.DeleteAlbum;
using Application.CQ.Admin.Artists.Command.DeleteArtist;
using Application.CQ.Admin.Songs.Command.DeleteSong;
using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Enums;
using Domain.Helpers;
using Domain.Primitives;
using Hangfire;
using MediatR;

namespace Api.Endpoints;

public static class AdminEndpoints
{
    /// <summary>
    /// Mostly util methods to bulk update broken data
    /// </summary>
    public static IEndpointRouteBuilder RegisterAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/admin").WithTags("Admin").RequireAuthorization(policy => policy.RequireRole(GlobalVariables.UserConstants.Roles.Admin));
        var utils = group.MapGroup("/utils");

        utils.MapPost("/artist", async (IYoutubeService _youtube, IUnitOfWork _uow) =>
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

        utils.MapPost("/albums", async (IYoutubeService _youtube, IUnitOfWork _uow, IStorageService _storageService) =>
        {
            var albumsToCheck = _uow.AlbumRepository
                .Where(x => string.IsNullOrEmpty(x.ThumbnailId))
                .ToList();
            
            foreach (var album in albumsToCheck)
            {
                var albumInfo = await _youtube.GetPlaylistInfoAsync(album.SourceId);
                var thumbnailId = albumInfo.Thumbnail?.Url;
                if (string.IsNullOrEmpty(thumbnailId)) continue;

                if (YoutubeHelper.IsYoutubeMusic(album.SourceId))
                {
                    var httpClient = new HttpClient();
                    await using var stream = await httpClient.GetStreamFromUrlAsync(thumbnailId);
                    thumbnailId = await _storageService.UploadFileAsync(stream, StorageFolder.Images);
                }

                album.ThumbnailId = thumbnailId;
                _uow.AlbumRepository.Update(album);
            }

            var rows =await _uow.SaveChangesAsync();
            return Results.Ok($"Updated {rows} records");
        });
        
        //Manually trigger file cleanup
        utils.MapPost("/cleanup", () => { RecurringJob.TriggerJob(FileCleanupJob.Id); });

        utils.MapDelete("/song/{guid:guid}", async (Guid guid, ISender sender) =>
        {
            var command = new DeleteSongCommand(guid);
            var result = await sender.Send(command);
            
            return result.IsSuccess 
                ? Results.NoContent() 
                : Results.BadRequest(result.Errors);
        });
        
        utils.MapDelete("/album/{guid:guid}", async (Guid guid, ISender sender) =>
        {
            var command = new DeleteAlbumCommand(guid);
            var result = await sender.Send(command);
            
            return result.IsSuccess 
                ? Results.NoContent() 
                : Results.BadRequest(result.Errors);
        });
        
        utils.MapDelete("/artist/{guid:guid}", async (Guid guid, ISender sender) =>
        {
            var command = new DeleteArtistCommand(guid);
            var result = await sender.Send(command);
            
            return result.IsSuccess 
                ? Results.NoContent() 
                : Results.BadRequest(result.Errors);
        });
        
        return app;
    }
}