using Application.DTOs.Playlists;
using Application.Repositories.Shared;
using AutoMapper;
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
            var plSongs = await _uow.PlaylistRepository.GetByGuidAsync(request.PlaylistGuid, asNoTracking: true, pl => pl.Songs, pl => pl.User);
            return _mapper.Map<PlaylistDTO>(plSongs);
        }
    }
}
