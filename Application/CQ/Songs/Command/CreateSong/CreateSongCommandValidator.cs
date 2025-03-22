using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;

namespace Application.CQ.Songs.Command.CreateSong
{
    public sealed class CreateSongCommandValidator : AbstractValidator<CreateSongCommand>
    {
        public CreateSongCommandValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.ArtistGuid).MustAsync(async (artistGuid, cancelationToken) =>
            {
                return await uow.ArtistRepository.ExistsAsync(x => x.Guid == artistGuid);
            }).WithMessage(Error.Exists(nameof(Artist)).Description);

            RuleFor(x => x.Title).Length(min: GlobalVariables.SongConstants.TitleMinLength, max: GlobalVariables.SongConstants.TitleMaxLength)
                .WithMessage(SongError.InvalidTitleLength.Description);

            RuleFor(x => x.AudioFileStream).Must((fs) =>
            {
                return fs.Length <= GlobalVariables.SongConstants.MaxSizeKB;
            }).WithMessage(SongError.InvalidSize.Description);
        }
    }
}
