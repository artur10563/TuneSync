using Application.DTOs.Playlists;
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

        public GetPlaylistsByUserCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<IEnumerable<PlaylistSummaryDTO>>> Handle(GetPlaylistsByUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserGuid == Guid.Empty)
                return Error.NotFound(nameof(Playlist));
            
            var userPlaylists = (
                from playlist in _uow.PlaylistRepository.NoTrackingQueryable()
                join favoredBy in _uow.UserFavoritePlaylistRepository.NoTrackingQueryable() 
                    on new { g = playlist.Guid, u = request.UserGuid } equals new { g = favoredBy.PlaylistGuid, u = favoredBy.UserGuid } into favoredJoin
                from favored in favoredJoin.DefaultIfEmpty()
                join songPlaylist in _uow.PlaylistSongRepository.NoTrackingQueryable()
                    on playlist.Guid equals songPlaylist.PlaylistGuid into songGroup
                where playlist.CreatedBy == request.UserGuid
                    select PlaylistSummaryDTO.Create(
                        playlist, 
                        favored != null && favored.IsFavorite, 
                        songGroup.Count())
            ).ToList();

            return userPlaylists;
        }
    }
}