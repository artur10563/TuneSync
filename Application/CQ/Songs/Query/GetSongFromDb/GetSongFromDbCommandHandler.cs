using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongFromDb
{
    internal class GetSongFromDbCommandHandler : IRequestHandler<GetSongFromDbCommand, Result<List<SongDTO>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ISearchService _searchService;

        public GetSongFromDbCommandHandler(IUnitOfWork uow, ISearchService searchService)
        {
            _uow = uow;
            _searchService = searchService;
        }

        public async Task<Result<List<SongDTO>>> Handle(GetSongFromDbCommand request, CancellationToken cancellationToken)
        {
            var userGuid = request.CurrentUserGuid ?? Guid.Empty;
            var songs = await _searchService.Search(request.query);

            return Result.Success(songs);
        }
    }
}