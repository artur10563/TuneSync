using Application.DTOs.Playlists;
using Application.Repositories.Shared;
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

        //test after playlist actually has songs :/
        public async Task<Result<PlaylistDTO>> Handle(GetPlaylistByIdCommand request, CancellationToken cancellationToken)
        {
            var plSongs = await _uow.PlaylistRepository.GetByGuidAsync(request.PlaylistGuid, includes: pl => pl.Songs);
            
            return null;
        }
    }
}
