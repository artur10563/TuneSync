using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Command;

public class ToggleFavoriteAlbumCommandHandler : IRequestHandler<ToggleFavoriteAlbumCommand, Result>
{
    public readonly IUnitOfWork _uow;


    public ToggleFavoriteAlbumCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(ToggleFavoriteAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await _uow.AlbumRepository.FirstOrDefaultAsync(x => x.Guid == request.AlbumGuid);
        if (album == null)
            return Error.NotFound(nameof(Album));

        var existingRecord = await _uow.UserFavoriteAlbumRepository
            .FirstOrDefaultAsync(x => x.AlbumGuid == request.AlbumGuid && x.UserGuid == request.UserGuid);

        if (existingRecord == null)
            _uow.UserFavoriteAlbumRepository.Insert(new UserFavoriteAlbum(request.UserGuid, request.AlbumGuid, isFavorite: true));
        else
        {
            existingRecord.IsFavorite = !existingRecord.IsFavorite;
        }

        await _uow.SaveChangesAsync();

        return Result.Success();
    }
}