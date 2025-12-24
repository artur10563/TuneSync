using Application.DTOs.Songs;
using Application.Extensions;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Playlists.Query.GetPlaylistSongsById;

internal sealed class GetPlaylistSongsByIdCommandHandler : IRequestHandler<GetPlaylistSongsByIdCommand, PaginatedResult<IEnumerable<SongDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;
    private readonly IValidator<GetPlaylistSongsByIdCommand> _validator;

    public GetPlaylistSongsByIdCommandHandler(IUnitOfWork uow, IProjectionProvider projectionProvider, IValidator<GetPlaylistSongsByIdCommand> validator)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
        _validator = validator;
    }

    public async Task<PaginatedResult<IEnumerable<SongDTO>>> Handle(GetPlaylistSongsByIdCommand request, CancellationToken cancellationToken)
    {
        var validationErrors = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationErrors.IsValid)
            return validationErrors.AsErrors(request.Page);

        var songsQuery = _uow.PlaylistRepository
            .Where(x => x.Guid == request.PlaylistGuid)
            .SelectMany(x => x.PlaylistSongs)
            .OrderByDescending(ps => ps.CreatedAt)
            .Select(ps => ps.Song);

        var songs = songsQuery
            .Select(_projectionProvider.GetSongWithArtistProjection(request.UserGuid))
            .Page(request.Page)
            .Select(SongDTO.FromProjection);
        
        var songsInfo = songsQuery.GetSongsInfo();

        return (songs, request.Page, GlobalVariables.PaginationConstants.PageSize, songsInfo.TotalCount, songsInfo.ToMetadataDictionary());
    }
}