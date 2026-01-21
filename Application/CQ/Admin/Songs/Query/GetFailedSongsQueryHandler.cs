using Application.DTOs.Songs;
using Application.Extensions;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Admin.Songs.Query;

public class GetFailedSongsQueryHandler : IRequestHandler<GetFailedSongsQuery, PaginatedResult<IEnumerable<SongDTO>>>
{
    private readonly IValidator<GetFailedSongsQuery> _validator;
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;

    public GetFailedSongsQueryHandler(IValidator<GetFailedSongsQuery> validator, IUnitOfWork uow, IProjectionProvider projectionProvider)
    {
        _validator = validator;
        _uow = uow;
        _projectionProvider = projectionProvider;
    }

    public async Task<PaginatedResult<IEnumerable<SongDTO>>> Handle(GetFailedSongsQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.AsErrors(GlobalVariables.PaginationConstants.PageSize);

        var songs = _uow.SongRepository.IgnoreFilter(CommonFilter.HasAudioFilter)
            .Where(x => x.AudioPath == null)
            .OrderBy(x => x.Title)
            .Page(request.Page)
            .Select(_projectionProvider.GetSongWithArtistProjection(request.UserGuid))
            .Select(x => SongDTO.FromProjection(x))
            .ToList();

        return (songs, request.Page, GlobalVariables.PaginationConstants.PageSize, songs.Count);
    }
}