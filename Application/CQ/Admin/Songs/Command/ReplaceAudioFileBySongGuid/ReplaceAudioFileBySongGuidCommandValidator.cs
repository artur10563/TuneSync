using Application.Extensions;
using Domain.Primitives;
using FluentValidation;

namespace Application.CQ.Admin.Songs.Command.ReplaceAudioFileBySongGuid;

public class ReplaceAudioFileBySongGuidCommandValidator : AbstractValidator<ReplaceAudioFileBySongGuidCommand>
{
    public ReplaceAudioFileBySongGuidCommandValidator()
    {
        RuleFor(x => x.SongGuid).NotEmpty();
        RuleFor(x => x.FileStream).Must(x => x.CanRead &&
                                             x.GetKilobytes() is > 0 and < GlobalVariables.SongConstants.MaxSizeKB);
    }
}