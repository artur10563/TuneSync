using Application.DTOs.Songs;
using Application.Extensions;
using Application.Projections;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongsMix;

public class GetSongsMixCommandHandler : IRequestHandler<GetSongsMixCommand, PaginatedResult<IEnumerable<SongDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ISearchService _searchService;
    private readonly IValidator<GetSongsMixCommand> _validator;
    private readonly IProjectionProvider _projectionProvider;

    public GetSongsMixCommandHandler(
        IUnitOfWork uow,
        ISearchService searchService,
        IValidator<GetSongsMixCommand> validator, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _searchService = searchService;
        _validator = validator;
        _projectionProvider = projectionProvider;
    }

    private double GetRandomDoubleFromSeed(string? seed = null)
    {
        seed = seed?.Trim();
        seed = string.IsNullOrEmpty(seed) ? Guid.NewGuid().ToString() : seed;
        return new Random(seed.GetHashCode()).NextDouble();
    }

    public async Task<PaginatedResult<IEnumerable<SongDTO>>> Handle(GetSongsMixCommand request, CancellationToken cancellationToken)
    {
        var validationErrors = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationErrors.IsValid)
            return validationErrors.AsErrors(request.Page);

        var userGuid = request.UserGuid ?? Guid.Empty;
        var songs = new List<SongDTO>();


        var artistChildren =
            _uow.ArtistRepository.Where(a => request.ArtistGuids.Contains(a.Guid),
                    includes: a => a.AllChildren,
                    asNoTracking: true)
                .SelectMany(a => a.AllChildren.Select(ch => ch.Guid));

        var allArtist = request.ArtistGuids.Union(artistChildren).ToList();

        //TODO: cache artist album? artist mix' are heavy to compute
        var baseQuery = _uow.SongRepository.NoTrackingQueryable().Where(s =>
            (request.AlbumGuids.Count != 0 && s.AlbumGuid.HasValue && request.AlbumGuids.Contains(s.AlbumGuid.Value))
            || (request.PlaylistGuids.Count != 0 && s.Playlists.Any(ps => request.PlaylistGuids.Contains(ps.Guid)))
            || (allArtist.Count != 0 && allArtist.Contains(s.ArtistGuid))
        ).Distinct();

        var songQuery = baseQuery.Select(_projectionProvider.GetSongWithArtistProjection(userGuid));

        //Transaction is required for ShuffleSeed to work 
        _uow.TransactedAction(() =>
        {
            songs = _searchService.Shuffle(songQuery, GetRandomDoubleFromSeed(request.ShuffleSeed))
                .Page(request.Page)
                .ToList()
                .Select(SongDTO.FromProjection)
                .ToList();
        });

        var songsInfo = baseQuery.GetSongsInfo();

        return new PaginatedResult<IEnumerable<SongDTO>>(songs, request.Page, songsInfo.TotalCount, songsInfo.ToMetadataDictionary());
    }
}