using Application.CommonValidators;

namespace Application.CQ.Admin.Songs.Query;

public sealed class GetFailedSongsQueryValidator : PagedRequestValidator<GetFailedSongsQuery>
{
    public GetFailedSongsQueryValidator()
    {
    }
}