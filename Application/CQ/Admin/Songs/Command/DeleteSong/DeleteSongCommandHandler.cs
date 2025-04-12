using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Admin.Songs.Command.DeleteSong;

public sealed class DeleteSongCommandHandler : IRequestHandler<DeleteSongCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IStorageService _storage;

    public DeleteSongCommandHandler(IUnitOfWork uow, IStorageService storage)
    {
        _uow = uow;
        _storage = storage;
    }

    /// <summary>
    /// Delete single song
    /// </summary>
    public async Task<Result> Handle(DeleteSongCommand request, CancellationToken cancellationToken)
    {
        var song = await _uow.SongRepository.FirstOrDefaultWithDependantAsync(x => x.Guid == request.SongGuid);

        if (song == null) return Error.NotFound(nameof(Song));

        var audioPath = song.GetAudioPath();

        _uow.SongRepository.Delete(song);

        if (await _uow.SaveChangesAsync() > 0)
        {
            await _storage.TryDeleteFileAsync(audioPath);
        }

        return Result.Success();
    }
}