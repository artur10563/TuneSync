using Application.CommonValidators;

namespace Application.CQ.Songs.Query.GetSongFromDb;

public sealed class GetSongFromDbCommandValidator : PagedRequestValidator<GetSongFromDbCommand>
{
    public GetSongFromDbCommandValidator()
    {
    }
}
