using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongFromDb
{
    internal class GetSongFromDbCommandHandler : IRequestHandler<GetSongFromDbCommand, Result<List<SongDTO>>>
    {
        private readonly IUnitOfWork _uow;

        public GetSongFromDbCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<List<SongDTO>>> Handle(GetSongFromDbCommand request, CancellationToken cancellationToken)
        {
            var userGuid = request.CurrentUserGuid ?? Guid.Empty;

            var songs = _uow.SongRepository.GetSongWith_Artist_Playlist_Favored()
                .Where(x => x.Title.ToLower().Contains(request.query.ToLower()))
                .Select(song => SongDTO.Create(song, userGuid))
                .ToList();


            return Result.Success(songs);
        }
    }
}