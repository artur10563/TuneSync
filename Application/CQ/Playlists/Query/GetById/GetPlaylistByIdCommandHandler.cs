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
        public Task<Result<PlaylistDTO>> Handle(GetPlaylistByIdCommand request, CancellationToken cancellationToken)
        {
            var pl = (from playlist in _uow.PlaylistRepository.Queryable()
                      select playlist
                      );
            //_uow.PlaylistRepository.Where(x => x.Guid == request.PlaylistGuid).FirstOrDefault();


            var pl1 = (from playlist in _uow.PlaylistRepository.Queryable()
                      select playlist.Songs
                      );
            return null;
        }
    }
}
