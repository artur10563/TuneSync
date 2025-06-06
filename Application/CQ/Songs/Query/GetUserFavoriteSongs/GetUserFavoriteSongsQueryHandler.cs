using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetUserFavoriteSongs;

public class GetUserFavoriteSongsQueryHandler : IRequestHandler<GetUserFavoriteSongsQuery, Result<IEnumerable<SongDTO>>>
{
    private readonly IUnitOfWork _uow;

    public GetUserFavoriteSongsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<IEnumerable<SongDTO>>> Handle(GetUserFavoriteSongsQuery request, CancellationToken cancellationToken)
    {
        var userFS = _uow.SongRepository
            .Where(s => s.FavoredBy
                    .Any(us => us.UserGuid == request.UserGuid && us.IsFavorite),
                asNoTracking:true,
                includes: [s => s.FavoredBy, s => s.Artist]
            ).OrderBy(x=>x.CreatedAt).Select(song => SongDTO.Create(song, song.Artist, true)).ToList();

        return userFS;
    }
}