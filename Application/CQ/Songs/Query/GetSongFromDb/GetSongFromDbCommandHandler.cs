using Application.DTOs.Songs;
using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongFromDb
{
    internal class GetSongFromDbCommandHandler : IRequestHandler<GetSongFromDbCommand, PaginatedResult<List<SongDTO>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ISearchService _searchService;
        private readonly IValidator<GetSongFromDbCommand> _validator;
        public GetSongFromDbCommandHandler(IUnitOfWork uow, ISearchService searchService, IValidator<GetSongFromDbCommand> validator)
        {
            _uow = uow;
            _searchService = searchService;
            _validator = validator;
        }

        public async Task<PaginatedResult<List<SongDTO>>> Handle(GetSongFromDbCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.AsErrors(GlobalVariables.PaginationConstants.PageSize);
            
            var userGuid = request.CurrentUserGuid ?? Guid.Empty;

            var (songs, totalItems) = await _searchService.Search(request.query, request.Page, userGuid);

            return Result.Success(songs, request.Page, GlobalVariables.PaginationConstants.PageSize, totalItems);
        }
    }
}