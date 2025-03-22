using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Command.DeletePlaylist;

public class DeletePlaylistCommandHandler : IRequestHandler<DeletePlaylistCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeletePlaylistCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeletePlaylistCommand request, CancellationToken cancellationToken)
    {
        if (request.PlaylistGuid == Guid.Empty) return Error.AccessDenied;

        var playlist = await _uow.PlaylistRepository
            .FirstOrDefaultAsync(x => x.Guid == request.PlaylistGuid && x.CreatedBy == request.UserGuid
                , includes: p => p.Songs); //Must include songs for cascade deletion

        if (playlist == null)
            return Error.NotFound(nameof(Playlist));

        _uow.PlaylistRepository.Delete(playlist);
        await _uow.SaveChangesAsync();

        return Result.Success();
    }
}