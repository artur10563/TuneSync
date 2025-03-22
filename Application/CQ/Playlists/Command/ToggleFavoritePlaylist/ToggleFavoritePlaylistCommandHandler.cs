using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Command.ToggleFavoritePlaylist;

public class ToggleFavoritePlaylistCommandHandler : IRequestHandler<ToggleFavoritePlaylistCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public ToggleFavoritePlaylistCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(ToggleFavoritePlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _uow.PlaylistRepository.FirstOrDefaultAsync(x => x.Guid == request.PlaylistGuid);
        if (playlist == null)
            return Error.NotFound(nameof(Playlist));

        var existingRecord = await _uow.UserFavoritePlaylistRepository
            .FirstOrDefaultAsync(x => x.PlaylistGuid == request.PlaylistGuid && x.UserGuid == request.UserGuid);

        if (existingRecord == null)
            _uow.UserFavoritePlaylistRepository.Insert(new UserFavoritePlaylist(request.UserGuid, request.PlaylistGuid, isFavorite: true));
        else
        {
            existingRecord.IsFavorite = !existingRecord.IsFavorite;
        }

        await _uow.SaveChangesAsync();

        return Result.Success();
    }
}