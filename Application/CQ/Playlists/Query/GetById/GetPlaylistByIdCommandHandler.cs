using Application.DTOs.Playlists;
using Application.DTOs.Songs;
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

        public GetPlaylistByIdCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<PlaylistDTO>> Handle(GetPlaylistByIdCommand request, CancellationToken cancellationToken)
        {
            var page = 1;
            var pageSize = 25;
            
            var userGuid = request.UserGuid ?? Guid.Empty;
            
            var playlistDetails = (await _uow.PlaylistRepository.FirstOrDefaultAsync(x => x.Guid == request.PlaylistGuid,
                asNoTracking: true,
                 p => p.User));

            if(playlistDetails == null)
                return Error.NotFound(nameof(Playlist));

            var playlistSongsQuery = _uow.SongRepository
                .Where(song => song.Playlists.Any(p => p.Guid == request.PlaylistGuid),
                    asNoTracking: true,
                    song => song.Album,
                    song => song.Artist,
                    song => song.FavoredBy).Select(x => SongDTO.Create(x, userGuid));
            
            var totalCount = playlistSongsQuery.Count();
            
            var playlistSongs = playlistSongsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToList();
            

            var paginatedPlaylistDTO = PlaylistDTO.Create(playlistDetails, playlistSongs,  PageInfo.Create(page, pageSize, totalCount));
            
            return paginatedPlaylistDTO;
        }
    }
}