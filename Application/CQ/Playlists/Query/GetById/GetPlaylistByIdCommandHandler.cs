using Application.DTOs.Playlists;
using Application.DTOs.Songs;
using Application.Extensions;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetById
{
    internal sealed class GetPlaylistByIdCommandHandler : IRequestHandler<GetPlaylistByIdCommand, Result<PlaylistDTO>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IProjectionProvider _projectionProvider;
        
        public GetPlaylistByIdCommandHandler(IUnitOfWork uow, IProjectionProvider projectionProvider)
        {
            _uow = uow;
            _projectionProvider = projectionProvider;
        }

        public async Task<Result<PlaylistDTO>> Handle(GetPlaylistByIdCommand request, CancellationToken cancellationToken)
        {
            var userGuid = request.UserGuid ?? Guid.Empty;
            
            //TODO: add pagination for playlists. All songs are pulled rn
            var playlist = _uow.PlaylistRepository
                .Where(x => x.Guid == request.PlaylistGuid, asNoTracking: true)
                .Select(_projectionProvider.GetPlaylistWithSongsProjection(userGuid))
                .FirstOrDefault();
            
            if(playlist == null)
                return Error.NotFound(nameof(Playlist));
            
            var playlistDto = PlaylistDTO.FromProjection(playlist);
            
            return playlistDto;
        }
    }
}