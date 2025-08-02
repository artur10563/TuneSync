using Application.CommonValidators;

namespace Application.CQ.Album.Query.GetAlbumSongsById;

public sealed class GetAlbumDetailsByIdQueryValidator : PagedRequestValidator<GetAlbumSongsByIdQuery>
{
    public GetAlbumDetailsByIdQueryValidator()
    {
    }
}