using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Admin.Songs.Command.ReplaceAudioFileBySongGuid;

internal sealed class ReplaceAudioFileBySongGuidCommandHandler : IRequestHandler<ReplaceAudioFileBySongGuidCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<ReplaceAudioFileBySongGuidCommand> _validator;
    private readonly ILoggerService _logger;
    private readonly ISongService _songService;

    public ReplaceAudioFileBySongGuidCommandHandler(IUnitOfWork uow, IValidator<ReplaceAudioFileBySongGuidCommand> validator, ILoggerService logger,
        ISongService songService)
    {
        _uow = uow;
        _validator = validator;
        _logger = logger;
        _songService = songService;
    }

    public async Task<Result<Guid>> Handle(ReplaceAudioFileBySongGuidCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.AsErrors();

        try
        {
            var song = await _uow.SongRepository.FirstOrDefaultAsync(x => x.Guid == request.SongGuid, ignoreFilters: true);
            if (song == null) return Error.NotFound(nameof(Song));

            var uploadResult = await _songService.ReplaceAudioFileAsync(song, GlobalVariables.AudioSource.File, request.FileStream);
            if (uploadResult.IsFailure)
            {
                return uploadResult.Errors;
            }

            return song.Guid;
        }
        catch (Exception e)
        {
            _logger.Log(e.Message, LogLevel.Error, context: request);
            return Error.SomethingWrong;
        }
    }
}