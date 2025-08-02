using Application.DTOs.Songs;
using Application.Extensions;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Album.Query.GetAlbumSongsById;

public class GetAlbumSongsByIdQueryHandler : IRequestHandler<GetAlbumSongsByIdQuery, PaginatedResult<IEnumerable<SongDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;
    private readonly IValidator<GetAlbumSongsByIdQuery> _validator;

    public GetAlbumSongsByIdQueryHandler(IUnitOfWork uow, IProjectionProvider projectionProvider, IValidator<GetAlbumSongsByIdQuery> validator)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
        _validator = validator;
    }

    public async Task<PaginatedResult<IEnumerable<SongDTO>>> Handle(GetAlbumSongsByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.AsErrors(GlobalVariables.PaginationConstants.PageSize);

        var userGuid = request.UserGuid ?? Guid.Empty;

        var albumSongsQuery = _uow.SongRepository
            .NoTrackingQueryable()
            .Where(s => s.AlbumGuid == request.AlbumGuid);

        var albumSongs = albumSongsQuery
            .OrderBy(x => x.CreatedAt)
            .Select(_projectionProvider.GetSongWithArtistProjection(userGuid))
            .Page(request.Page)
            .ToList()
            .Select(SongDTO.FromProjection);

        var songsInfo = albumSongsQuery.GetSongsInfo();

        return (albumSongs, request.Page, GlobalVariables.PaginationConstants.PageSize, songsInfo.TotalCount, songsInfo.ToMetadataDictionary());
    }
}