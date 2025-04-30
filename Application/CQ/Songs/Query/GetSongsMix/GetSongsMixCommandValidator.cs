using Application.Repositories.Shared;
using Domain.Errors;
using FluentValidation;
using static Domain.Primitives.GlobalVariables.MixConstants;

namespace Application.CQ.Songs.Query.GetSongsMix;

public class GetSongsMixCommandValidator : AbstractValidator<GetSongsMixCommand>
{
    public GetSongsMixCommandValidator(IUnitOfWork _uow)
    {
        RuleFor(x => x.page)
            .Must(page => page > 0)
            .WithMessage(PageError.InvalidPage.Description);
        
        //Min 2, max 50 unique Albums
        RuleFor(x => x).CustomAsync(async (x, context, cancellationToken) =>
        {
            if (x.ArtistGuids.Count > 0) return; //Stop further validation as single artist in enough for mix
            
            var uniquePlaylistGuids = await _uow.PlaylistRepository.GetUniqueExistingGuidsAsync(x.PlaylistGuids);
            var uniqueAlbumGuids = await _uow.AlbumRepository.GetUniqueExistingGuidsAsync(x.AlbumGuids);

            var totalCount = uniquePlaylistGuids.Count + uniqueAlbumGuids.Count;

            switch (totalCount)
            {
                case < MinCount:
                    context.AddFailure($"At least {MinCount} items (albums or playlists) are required.");
                    break;
                case > MaxCount:
                    context.AddFailure($"You can select up to {MaxCount} items (albums or playlists).");
                    break;
            }
        });
    }
}