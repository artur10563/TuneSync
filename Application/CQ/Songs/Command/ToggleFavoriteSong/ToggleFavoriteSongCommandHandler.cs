using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Command;

public class ToggleFavoriteSongCommandHandler : IRequestHandler<ToggleFavoriteSongCommand, Result>
{
    public readonly IUnitOfWork _uow;


    public ToggleFavoriteSongCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(ToggleFavoriteSongCommand request, CancellationToken cancellationToken)
    {
        var song = await _uow.SongRepository.FirstOrDefaultAsync(x => x.Guid == request.SongGuid);
        if (song == null)
            return Error.NotFound(nameof(Song));

        var existingRecord = await _uow.UserSongRepository
            .FirstOrDefaultAsync(x => x.SongGuid == request.SongGuid && x.UserGuid == request.UserGuid);

        if (existingRecord == null)
            _uow.UserSongRepository.Insert(new UserSong(request.SongGuid, request.UserGuid, isFavorite: true, isOffline: false));
        else
        {
            existingRecord.IsFavorite = !existingRecord.IsFavorite;
            existingRecord.CreatedAt = DateTime.UtcNow;
        }

        await _uow.SaveChangesAsync();

        return Result.Success();
    }
}