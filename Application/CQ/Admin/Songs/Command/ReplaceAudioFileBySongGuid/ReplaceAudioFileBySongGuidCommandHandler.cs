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
    private readonly IStorageService _storageService;
    private readonly IAudioMetadataReaderService _audioMetadataReaderService;
    private readonly ILoggerService _logger;

    public ReplaceAudioFileBySongGuidCommandHandler(IUnitOfWork uow, IValidator<ReplaceAudioFileBySongGuidCommand> validator, IStorageService storageService, IAudioMetadataReaderService audioMetadataReaderService, ILoggerService logger)
    {
        _uow = uow;
        _validator = validator;
        _storageService = storageService;
        _audioMetadataReaderService = audioMetadataReaderService;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(ReplaceAudioFileBySongGuidCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.AsErrors();

        var song = await _uow.SongRepository.FirstOrDefaultAsync(x => x.Guid == request.SongGuid, ignoreFilters: true);
        if (song == null) return Error.NotFound(nameof(Song));

        try
        {
            var (fileGuid, filePath) = await _storageService.UploadFileAsync(request.FileStream, StorageFolder.Audio);
            
            song.AudioSize = (int)request.FileStream.GetKilobytes();
            song.AudioSource = GlobalVariables.SongSource.File;
            song.AudioLength = _audioMetadataReaderService.GetMp3Duration(request.FileStream);
            song.AudioPath = fileGuid;
            _uow.SongRepository.Update(song);
            await _uow.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Log(e.Message, LogLevel.Error, context: request);
            return Error.SomethingWrong;
        }

        return song.Guid;
    }
}