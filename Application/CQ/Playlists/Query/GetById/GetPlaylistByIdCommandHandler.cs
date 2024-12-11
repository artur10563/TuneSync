using Application.DTOs.Playlists;
using Application.DTOs.Songs;
using Application.Repositories.Shared;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetById
{
    internal sealed class GetPlaylistByIdCommandHandler : IRequestHandler<GetPlaylistByIdCommand, Result<PlaylistDTO>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetPlaylistByIdCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<PlaylistDTO>> Handle(GetPlaylistByIdCommand request, CancellationToken cancellationToken)
        {
            var playlistDetails = (await _uow.PlaylistRepository.FirstOrDefaultAsync(x => x.Guid == request.PlaylistGuid,
                asNoTracking: true,
                includes: p => p.User));

            if(playlistDetails == null)
                return Error.NotFound(nameof(Playlist));
            
            var playlistSongs =
                (
                    from playlistSong in _uow.PlaylistSongRepository.Queryable()
                    join song in _uow.SongRepository.Queryable()
                        on playlistSong.SongGuid equals song.Guid
                    where playlistSong.PlaylistGuid == request.PlaylistGuid
                    select song
                ).ProjectTo<SongDTO>(_mapper.ConfigurationProvider)
                .ToList();
            
            var playlistDto = _mapper.Map<PlaylistDTO>(playlistDetails);
            playlistDto.Songs = playlistSongs;
            

            return playlistDto;
        }
    }
}