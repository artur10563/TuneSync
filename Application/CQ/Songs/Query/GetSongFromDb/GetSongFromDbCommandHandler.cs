using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongFromDb
{
    internal class GetSongFromDbCommandHandler : IRequestHandler<GetSongFromDbCommand, PaginatedResult<List<SongDTO>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ISearchService _searchService;

        public GetSongFromDbCommandHandler(IUnitOfWork uow, ISearchService searchService)
        {
            _uow = uow;
            _searchService = searchService;
        }

        public async Task<PaginatedResult<List<SongDTO>>> Handle(GetSongFromDbCommand request, CancellationToken cancellationToken)
        {
            var userGuid = request.CurrentUserGuid ?? Guid.Empty;

            if (request.page <= 0) return Result.Failure<List<SongDTO>>(new Error("Invalid page number"), 0);
            
            var (songs, totalItems) = await _searchService.Search(request.query, request.page, userGuid);

            return Result.Success(songs, request.page, totalItems);
        }
    }
}