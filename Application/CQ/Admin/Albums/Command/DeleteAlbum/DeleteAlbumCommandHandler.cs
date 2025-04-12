using Application.Repositories.Shared;
using Application.Services;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Admin.Albums.Command.DeleteAlbum;

public class DeleteAlbumCommandHandler : IRequestHandler<DeleteAlbumCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IStorageService _storage;

    public DeleteAlbumCommandHandler(IUnitOfWork uow, IStorageService storage)
    {
        _uow = uow;
        _storage = storage;
    }

    /// <summary>
    /// Delete album and its songs
    /// </summary>
    public async Task<Result> Handle(DeleteAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await _uow.AlbumRepository.FirstOrDefaultWithDependantAsync(album => album.Guid == request.AlbumGuid);
        
        if (album == null) return Error.NotFound(nameof(Album));

        var imagePath = album.ThumbnailSource == GlobalVariables.PlaylistSource.YouTubeMusic
            ? album.ThumbnailId
            : null;
        
        var audioFiles = album.Songs
            .Select(x => x.GetAudioPath())
            .ToList();
        
        _uow.AlbumRepository.Delete(album);
        
        if (await _uow.SaveChangesAsync() > 0)
        {
            if(imagePath != null) 
                await _storage.TryDeleteFileAsync(imagePath);
            
            var deleteTasks = audioFiles.Select(f => _storage.TryDeleteFileAsync(f));
            await Task.WhenAll(deleteTasks);
            
        }
        
        return Result.Success();
    }
}