using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Admin.Artists.Command.DeleteArtist;

public class DeleteArtistCommandHandler : IRequestHandler<DeleteArtistCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IStorageService _storage;

    public DeleteArtistCommandHandler(IUnitOfWork uow, IStorageService storage)
    {
        _uow = uow;
        _storage = storage;
    }

    /// <summary>
    /// Delete  artist, his albums, all songs of these albums
    /// </summary>
    public async Task<Result> Handle(DeleteArtistCommand request, CancellationToken cancellationToken)
    {
        var artist = await _uow.ArtistRepository.FirstOrDefaultWithDependantAsync(x => x.Guid == request.ArtistGuid);

        if (artist == null) return Error.NotFound(nameof(Artist));

        var files = artist.Albums
            .Where(x => x.ThumbnailSource == GlobalVariables.PlaylistSource.YouTubeMusic)
            .Select(x => x.ThumbnailId)
            .Union(
                artist.Albums
                    .SelectMany(a => a.Songs)
                    .Select(x => x.GetAudioPath())
            );

        _uow.ArtistRepository.Delete(artist);
        
        if (await _uow.SaveChangesAsync() > 0)
        {
            var deleteTasks = files.Select(f => _storage.TryDeleteFileAsync(f));
            await Task.WhenAll(deleteTasks);
        }
        return Result.Success();
    }
}