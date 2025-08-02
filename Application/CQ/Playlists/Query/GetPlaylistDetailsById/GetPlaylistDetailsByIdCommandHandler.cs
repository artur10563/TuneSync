using Application.DTOs.Playlists;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetById
{
    internal sealed class GetPlaylistDetailsByIdCommandHandler : IRequestHandler<GetPlaylistDetailsByIdCommand, Result<PlaylistDTO>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IProjectionProvider _projectionProvider;
        
        public GetPlaylistDetailsByIdCommandHandler(IUnitOfWork uow, IProjectionProvider projectionProvider)
        {
            _uow = uow;
            _projectionProvider = projectionProvider;
        }

        public async Task<Result<PlaylistDTO>> Handle(GetPlaylistDetailsByIdCommand request, CancellationToken cancellationToken)
        {
            var userGuid = request.UserGuid ?? Guid.Empty;
            
            var playlist = _uow.PlaylistRepository
                .Where(x => x.Guid == request.PlaylistGuid, asNoTracking: true)
                .Select(_projectionProvider.GetPlaylistProjection(userGuid))
                .FirstOrDefault();
            
            if(playlist == null)
                return Error.NotFound(nameof(Playlist));
            
            var playlistDto = PlaylistDTO.FromProjection(playlist);
            
            return playlistDto;
        }
    }
}