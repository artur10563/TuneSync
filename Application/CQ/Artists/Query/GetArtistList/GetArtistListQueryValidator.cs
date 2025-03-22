using Domain.Primitives;
using FluentValidation;

namespace Application.CQ.Album.Query.GetArtistList;

public class GetArtistListQueryValidator : AbstractValidator<GetArtistListQuery>
{
    public GetArtistListQueryValidator()
    {
        RuleFor(x => x.OrderBy).Must((orderBy) => GlobalVariables.ArtistConstants.ArtistSortColumns.Contains(orderBy)).WithMessage("Invalid sorting column");
    }
}