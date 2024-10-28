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
            var result = (from song in _uow.SongRepository.Queryable()
                          where song.Guid.ToString() == request.query || song.Title.Contains(request.query) || song.Artist.Contains(request.query)
                          select new SongDTO
                          {
                              Guid = song.Guid,
                              Title = song.Title,
                              Artist = song.Artist,
                              VideoId = song.SourceId,
                              AudioPath = song.AudioPath,
                              AudioLength = song.AudioLength,
                              AudioSize = song.AudioSize,
                              CreatedAt = song.CreatedAt,
                              IsFavorite = false
                          }).ToList();

            return Result.Success(result);
        }
    }
}
