using Application.Repositories.Shared;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Command.DeleteSongFromPlaylist;

public class DeleteSongFromPlaylistCommandHandler : IRequestHandler<DeleteSongFromPlaylistCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteSongFromPlaylistCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteSongFromPlaylistCommand request, CancellationToken cancellationToken)
    {
        var playlist = await _uow.PlaylistRepository
            .FirstOrDefaultAsync(x => x.Guid == request.PlaylistGuid && x.CreatedBy == request.CurrentUserGuid 
                                                                     && x.Source == GlobalVariables.PlaylistSource.User);
        if (playlist == null)
            return Error.NotFound(nameof(playlist));

        var ps = await _uow.PlaylistSongRepository
            .FirstOrDefaultAsync(x => x.PlaylistGuid == playlist.Guid && x.SongGuid == request.SongGuid);

        if (ps is null)
            return SongError.SongNotInPlaylist;
        
        _uow.PlaylistSongRepository.Delete(ps);
        await _uow.SaveChangesAsync();
        
        return Result.Success();
    }
}