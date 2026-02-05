using Application.DTOs.Songs;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongByGuid;

public class GetSongByGuidCommandHandler : IRequestHandler<GetSongByGuidCommand, Result<SongDTO>>
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;

    public GetSongByGuidCommandHandler(IUnitOfWork uow, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
    }


    public async Task<Result<SongDTO>> Handle(GetSongByGuidCommand request, CancellationToken cancellationToken)
    {
        var song = _uow.SongRepository
            .Where(x => x.Guid == request.SongGuid)
            .Select(_projectionProvider.GetSongWithArtistProjection(request.UserGuid))
            .FirstOrDefault();

        if (song == null) return Error.NotFound(nameof(Song));
            
        return SongDTO.FromProjection(song);
    }
}