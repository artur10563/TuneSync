using Application.DTOs.Playlists;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetPlaylistsByUser
{
    internal sealed class GetPlaylistsByUserCommandHandler : IRequestHandler<GetPlaylistsByUserCommand, Result<IEnumerable<PlaylistSummaryDTO>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IProjectionProvider _projectionProvider;
        
        public GetPlaylistsByUserCommandHandler(IUnitOfWork uow, IProjectionProvider projectionProvider)
        {
            _uow = uow;
            _projectionProvider = projectionProvider;
        }

        public async Task<Result<IEnumerable<PlaylistSummaryDTO>>> Handle(GetPlaylistsByUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserGuid == Guid.Empty)
                return Error.NotFound(nameof(Playlist));
            
            var userPlaylists = _uow.PlaylistRepository
                .NoTrackingQueryable()
                .Select(_projectionProvider.GetPlaylistSummaryProjection(request.UserGuid))
                .Select(x => PlaylistSummaryDTO.FromProjection(x))
                .ToList();

            return userPlaylists;
        }
    }
}